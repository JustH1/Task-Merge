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
		public async Task<IActionResult> OnPost(string email, string passwd) 
		{
			customer user = (customer)db.users.FromSqlInterpolated($"select * from user where email{email}");
			if (user != null)
			{
				if (user.password == HashPasswd(passwd))
				{
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, user.name),
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
