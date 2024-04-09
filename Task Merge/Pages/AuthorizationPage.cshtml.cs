using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Task_Merge.Pages
{
    public class AuthorizationPageModel : PageModel
    {
		private TaskMergeDB db;
		private TaskMergeRole role;
		public AuthorizationPageModel(TaskMergeDB db, TaskMergeRole role)
		{
			this.db = db;
			this.role = role;
		}
		public void OnGet() {}
		public async Task<IActionResult> OnPost(string email, string password) 
		{
			try
			{
				customer user = db.customer.FromSqlInterpolated($"select * from customer where email={email}").First();
				if (user != null)
				{
					if (user.password == HashPasswd(password))
					{
						var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, user.id.ToString()),
						new Claim(ClaimTypes.Role, user.user_type)
					};
						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
						return Redirect("/");
					}
					else
					{
						return Content("Invalid password.");
					}
				}
				else
				{
					return Content("The user is missing register.");
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
