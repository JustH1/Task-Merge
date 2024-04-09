using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Task_Merge.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private TaskMergeDB db;
        private TaskMergeRole role;

        public List<task> tasks { get; set; }

		public IndexModel(TaskMergeDB db, TaskMergeRole role)
        {
            this.db = db;
            this.role = role;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
				IIdentity? userIdentity = HttpContext.User.Identity;
				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
                    tasks = db.task.Where(p => p.student_id == Convert.ToInt32(userIdentity.Name)).ToList();
				}
				else
				{
                    return Unauthorized();
				}
			}
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Content(ex.Message);
            }
        }
    }
}