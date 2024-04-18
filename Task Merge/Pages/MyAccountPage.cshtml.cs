using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Task_Merge.Pages
{
    public class MyAccountPageModel : PageModel
    {
        private TaskMergeDB db;

        public customer customer_view { get; set; }

        public MyAccountPageModel(TaskMergeDB db)
        {
            this.db = db;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                IIdentity? userIdentity = HttpContext.User.Identity;
                if (userIdentity != null && userIdentity.IsAuthenticated)
                {
                    customer_view = db.customer.Where(p => p.id.ToString() == userIdentity.Name).First();
                    return Page();
                }
                else
                {
                    return Redirect("Index");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        public async Task<IActionResult> OnPost(string? username, string? email, string password, string repeat_password)
        {
            try
            {
                IIdentity? userIdentity = HttpContext.User.Identity;
                if (userIdentity != null && userIdentity.IsAuthenticated)
                {
                    int userId = Convert.ToInt32(userIdentity.Name);

                    if (password == repeat_password)
                    {
                        if (username == null && email == null)
                        {
                            return OnGet().Result;
                        }
                        else
                        {
                            customer currentCustomer = db.customer.FirstOrDefault(p => p.id == userId);
                            if (HashPasswd(password) == currentCustomer.password)
                            {
								if (username != null && email != null)
								{
                                    currentCustomer.name = username;
                                    currentCustomer.email = email;
                                }
								else if (email != null)
								{
									currentCustomer.email = email;
								}
								else
								{
                                    currentCustomer.name = username;
								}
								db.SaveChanges();
								customer_view = currentCustomer;
								return Page();
							}
                            else
                            {
                                return StatusCode(400);
                            }                            
                        }
                    }
                    else
                    {
                        return StatusCode(400);
                    }
                    
                }
                else
                {
                    return Redirect("Index");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
		private string HashPasswd(string passwd)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
				string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
				return hashedPassword;
			}
		}
	}
}
