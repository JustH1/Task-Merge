using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Task_Merge.Pages
{
    public class DownloadFilePageModel : PageModel
    {
		private TaskMergeDB db;
		private ILogger<DownloadFilePageModel> logger;
		public DownloadFilePageModel(TaskMergeDB db, ILogger<DownloadFilePageModel> logger)
		{
			this.db = db;
			this.logger = logger;
		}
		public async Task<IActionResult> OnGet(string fileName, string taskId)
        {
			try
			{
				IIdentity? userIdentity = HttpContext.User.Identity;
				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
					task task = db.task.Where(p => p.id == taskId).First();

					if (userIdentity.Name == task.student_id.ToString() || userIdentity.Name == task.teacher_id.ToString())
					{
						string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Downloaded_Files",fileName);
						string contentType = "text/plain";
						return PhysicalFile(filePath, contentType, fileName);
					}
					else
					{
						return StatusCode(404);
					}
				}
				else
				{
					return StatusCode(404);
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
