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
        public async Task<IActionResult> OnPost(string username, string email, string password, string repeat_password) 
        {
            if (db.users.FromSqlInterpolated($"select id from user where email={email};") == null)
            {
				if (password == repeat_password)
				{
                    var user = new customer()
                    {
                        name = username,
                        phone = email,
                        password = HashPasswd(password)
                    };
					db.users.Add(user);

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, user.name),
                        new Claim(ClaimTypes.Role, role.student.Name)
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
