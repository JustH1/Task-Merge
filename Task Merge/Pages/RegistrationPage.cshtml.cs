using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Task_Merge.Pages
{
    public class RegistrationPageModel : PageModel
    {
        private TaskMergeDB db;
        private TaskMergeRole role;
        public RegistrationPageModel(TaskMergeDB db, TaskMergeRole role)
        {
            this.db = db;
            this.role = role;
        } 
        public void OnGet() {}
        public async Task<IActionResult> OnPost(string username, string email, string password, string repeat_password, string role) 
        {
            try
            {
				customer[] customer1 = db.customer.FromSqlInterpolated($"select * from customer where email = \'{email}\'").ToArray();

				if (!(customer1.Length > 0))
				{

					if (password == repeat_password)
					{
						var customer = new customer()
						{
							name = username,
							email = email,
							password = HashPasswd(password),
							user_type = role.ToLower()
						};

						db.customer.Add(customer);
						db.SaveChanges();

						string id = db.customer.FromSqlInterpolated($"select * from customer where email={customer.email}").First().id.ToString();
						var claims = new List<Claim>
						{
						new Claim(ClaimTypes.Name, id),
						new Claim(ClaimTypes.Role, customer.user_type)
						};
						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
						return Redirect("/");
					}
					else
					{
						return Content("Passwords don't match.");
					}
				}
				else
				{
					return Content("The account already exists.");
				}
			}
            catch (Exception ex)
            {
				HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				return Content(ex.Message);
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
