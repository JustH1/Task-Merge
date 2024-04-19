using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Task_Merge.Pages
{
    public class RatingTaskPageModel : PageModel
    {
		private TaskMergeDB db;
		private ILogger<RatingTaskPageModel> logger;

        public task currentTask { get; set; }
        public customer teacher { get; set; }
        public customer student { get; set; }
        public List<string> taskFileNames { get; set; } = new List<string>();


        public RatingTaskPageModel(TaskMergeDB db, ILogger<RatingTaskPageModel> logger)
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
					if (HttpContext.User.IsInRole("teacher"))
					{

						task currentTask = db.task.FirstOrDefault(p => p.id == RouteData.Values["id"]);

						if (currentTask.teacher_id.ToString() == userIdentity.Name)
						{
							this.currentTask = currentTask;
                            student = db.customer.Where(p => p.id == currentTask.student_id).First();
                            teacher = db.customer.Where(p => p.id == currentTask.teacher_id).First();
                            task_files[] paths = db.task_files.Where(p => p.task_id == RouteData.Values["id"]).ToArray();

                            foreach (var item in paths)
                            {
                                taskFileNames.Add(Path.GetFileName(item.file_path));
                            }

                            return Page();
						}
						else
						{
							return StatusCode(404);
						}
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
		public async Task<IActionResult> OnPost(short mark, string comment)
		{
			try
			{
				IIdentity? userIdentity = HttpContext.User.Identity;
				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
					if (HttpContext.User.IsInRole("teacher"))
					{

						task currentTask = db.task.FirstOrDefault(p => p.id == RouteData.Values["id"]);
						this.currentTask = currentTask;

						if (currentTask.teacher_id.ToString() == userIdentity.Name)
						{
							currentTask.mark = mark;
							currentTask.comments_checking_task = comment;
							db.SaveChanges();

							return RedirectToPage("TeacherPage");
						}
						else
						{
							return StatusCode(404);
						}
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
