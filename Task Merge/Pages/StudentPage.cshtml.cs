using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace Task_Merge.Pages
{
	public class StudentPage : PageModel
    {
        private TaskMergeDB db;
        private ILogger<StudentPage> logger;

        public string userName { get; set; }
        public List<task> tasks { get; set; }

		public StudentPage(TaskMergeDB db, ILogger<StudentPage> logger)
        {
            this.db = db;
            this.logger = logger;
        }
        public async Task<IActionResult> OnGet()
        {
			try
            {
				IIdentity? userIdentity = HttpContext.User.Identity;
				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
                    if (HttpContext.User.IsInRole("student"))
                    {
						userName = db.customer.Where(p => p.id.ToString() == userIdentity.Name).First().name;
						tasks = db.task.Where(p => p.student_id == Convert.ToInt32(userIdentity.Name) && p.status == false).ToList();
						return Page();
					}
                    else { return StatusCode(404); }
				}
                else
                {
                    return Redirect("Index");
                }
			}
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                return StatusCode(500);
            }  
		}
	}
}