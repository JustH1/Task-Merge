using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using System.Security.Principal;

namespace Task_Merge.Pages
{
    public class TeacherPage : PageModel
    {
        private TaskMergeDB db;
        private TaskMergeRole role;
        private readonly IWebHostEnvironment hostingEnvironment;

        public string userName { get; set; }
        public List<task> tasks { get; set; }

        public TeacherPage(TaskMergeDB db, TaskMergeRole role, IWebHostEnvironment hostingEnvironment)
        {
            this.db = db;
            this.role = role;
            this.hostingEnvironment = hostingEnvironment;
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
                        userName = db.customer.Where(p => p.id.ToString() == userIdentity.Name).First().name;
						tasks = db.task.Where(p => p.teacher_id == Convert.ToInt32(userIdentity.Name)).ToList();
						return Page();
					}
					else { return StatusCode(404); }	
                }
                else
                {
                    return Redirect("Index");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        public async Task<IActionResult> OnPost(string student_id, string task_content, string task_time, string task_date, IFormFileCollection? files)
        {
            try
            {
                IIdentity? userIdentity = HttpContext.User.Identity;
                if (userIdentity != null && userIdentity.IsAuthenticated)
                {
                    Guid guid = Guid.NewGuid();
                    string uniqueId = guid.ToString().Replace("-", "").Substring(0, 10);
                        
                    task currentTask = new task()
                    {
                        id = uniqueId,
                        content = task_content,
                        status = false,
                        teacher_id = Convert.ToInt32(userIdentity.Name),
                        time_start = DateTime.Now.ToString(),
                        time_end = DateTime.Parse($"{task_date} {task_time}").ToString()
                    };
                    currentTask.student_id = Convert.ToInt32(student_id);

					db.task.Add(currentTask);
					db.SaveChanges();

					tasks = db.task.Where(p => p.teacher_id == Convert.ToInt32(userIdentity.Name)).ToList();

                    if (files != null)
                    {
                        foreach (var path in await DownloadFile(files, uniqueId))
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
                    
                    return Page();
                }
                else
                {
                    return Redirect("Index");
                }
            }
            catch (Exception)
            {
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
            return path;
        }
        private string GetFilesPaths(IFormFile file, string taskID,int count)
        {
            string fileName = file.FileName;
            string filePath = Path.Combine(hostingEnvironment.ContentRootPath, "Downloaded_Files",$"{taskID}({count})." + fileName.Split('.')[1]);
            return filePath;
        }
    }
}
