using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml;

namespace Task_Merge.Pages
{
    public class CreateAnswerPage : PageModel
    {
		private TaskMergeDB db;
		private ILogger<TeacherPage> logger;


		public task currentTask {  get; set; }
		public customer teacher { get; set; }
		public customer student { get; set; }
		public List<string> taskFileNames { get; set; } = new List<string>();

		public CreateAnswerPage(TaskMergeDB db, ILogger<TeacherPage> logger)
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

					task task = db.task.Where(p => p.id == RouteData.Values["id"]).First();

					if (userIdentity.Name == task.student_id.ToString())
					{
						currentTask = task;
						student = db.customer.Where(p => p.id == task.student_id).First();
						teacher = db.customer.Where(p => p.id == task.teacher_id).First();
						task_files[] paths = db.task_files.Where(p => p.task_id == RouteData.Values["id"]).ToArray();

						foreach (var item in paths)
						{
							taskFileNames.Add(Path.GetFileName(item.file_path));
						}

						return Page();
					}
					else
					{
						return StatusCode(400);
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
		public async Task<IActionResult> OnPost(IFormFileCollection? files)
		{
			try
			{
				IIdentity? userIdentity = HttpContext.User.Identity;

				if (userIdentity != null && userIdentity.IsAuthenticated)
				{
					
					task task = db.task.FirstOrDefault(p => p.id == RouteData.Values["id"]);
					currentTask = task;
					student = db.customer.Where(p => p.id == task.student_id).First();
					teacher = db.customer.Where(p => p.id == task.teacher_id).First();

                    if (userIdentity.Name == task.student_id.ToString() || userIdentity.Name == task.teacher_id.ToString())
					{
                        task.status = true;
                        db.SaveChanges();

                        if (files != null)
						{
                            foreach (var path in await DownloadFile(files, RouteData.Values["id"].ToString()))
							{
								task_files task_Files = new task_files()
								{
									task_id = currentTask.id,
									file_path = path
								};

								db.task_files.Add(task_Files);
							}
                            db.SaveChanges();
						}
						return RedirectToPage("StudentPage");
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
			catch (Exception ex)
			{
				logger.LogCritical(ex.Message);
				return StatusCode(400);
			}
		}
		private async Task<List<string>> DownloadFile(IFormFileCollection files, string uniqueId)
		{
			List<string> path = new List<string>();
			for (int i = 0; i < files.Count; i++)
			{
				string currentFilePath = GetFilesPaths(files[i], uniqueId, i);
				using (var stream = new FileStream(currentFilePath, FileMode.Create))
				{
					await files[i].CopyToAsync(stream);
				}
				path.Add(currentFilePath);
			}
			Console.WriteLine(path.Count);
			return path;
		}
		private string GetFilesPaths(IFormFile file, string taskID, int count)
		{
			string fileName = file.FileName;
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloaded_Files", $"Answer{taskID}({count})." + fileName.Split('.')[1]);
			return filePath;
		}

	}
}
