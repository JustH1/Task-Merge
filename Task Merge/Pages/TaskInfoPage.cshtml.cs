using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Task_Merge.Pages
{
    public class TaskInfoPageModel : PageModel
    {
		private TaskMergeDB db;

		public task currentTask {  get; set; }
		public customer teacher { get; set; }
		public customer student { get; set; }
		public List<string> taskFileNames { get; set; } = new List<string>();
		public bool checkRole { get; set; } //teacher = 1 student = 0

		public TaskInfoPageModel(TaskMergeDB db)
		{
			this.db = db;
		}
		public async Task<IActionResult> OnGet(string taskId)
        {
			try
			{
				IIdentity? userIdentity = HttpContext.User.Identity;
				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
					if (HttpContext.User.IsInRole("teacher")) {checkRole = true;}
					else { checkRole = false;}

					task task = db.task.Where(p => p.id == taskId).First();


					if (userIdentity.Name == task.student_id.ToString() || userIdentity.Name == task.teacher_id.ToString())
					{


						currentTask = task;
						student = db.customer.Where(p => p.id == task.student_id).First();
						teacher = db.customer.Where(p => p.id == task.teacher_id).First();
						task_files[] paths = db.task_files.Where(p => p.task_id == taskId).ToArray();

						foreach (var item in paths)
						{
							taskFileNames.Add(Path.GetFileName(item.file_path));
						}

						return Page();
					}
					else
					{
						return Redirect("Index");
					}
				}
				else
				{
					return StatusCode(404);
				}
			}
			catch (Exception)
			{
				return StatusCode(500);
			}
		}
    }
}
