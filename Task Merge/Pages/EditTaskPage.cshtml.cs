using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.IO;

namespace Task_Merge.Pages
{
    public class EditTaskPageModel : PageModel
    {
        private TaskMergeDB db;
        private ILogger<EditTaskPageModel> logger;

        public task currentTask { get; set; }
        public customer teacher { get; set; }
        public customer student { get; set; }
        public List<string> taskFileNames { get; set; } = new List<string>();

        public EditTaskPageModel(TaskMergeDB db, ILogger<EditTaskPageModel> logger)
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
                        task task = db.task.Where(p => p.id == RouteData.Values["id"]).First();

                        if (userIdentity.Name == task.teacher_id.ToString())
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
        public async Task<IActionResult> OnPost(IFormFileCollection? files, string newContent)
        {
            try
            {
                IIdentity? userIdentity = HttpContext.User.Identity;

                if (userIdentity != null && userIdentity.IsAuthenticated)
                {
					task task = db.task.FirstOrDefault(p => p.id == RouteData.Values["id"]);
					if (userIdentity.Name == task.teacher_id.ToString())
                    {
                                            
                        task.content = newContent;

                        if (files != null)
                        {
							foreach (var item in db.task_files.Where(p => p.task_id == RouteData.Values["id"]))
							{
								DeleteFile(item.file_path);
								db.task_files.Remove(item);
							}
							foreach (var path in await DownloadFile(files, RouteData.Values["id"].ToString()))
                            {
								task_files task_Files = new task_files()
                                {
                                    task_id = RouteData.Values["id"].ToString(),
                                    file_path = path
                                };

                                db.task_files.Add(task_Files);
                            }

							db.SaveChanges();
                        }
                        else
                        {
                            
                        }
                        return RedirectToPage("TeacherPage");
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
        private void DeleteFile(string Path)
        {
            System.IO.File.Delete(Path);
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
            Console.WriteLine(files.Count);
            return path;
        }
        private string GetFilesPaths(IFormFile file, string taskID, int count)
        {
            string fileName = file.FileName;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloaded_Files", $"Changed{taskID}({count})." + fileName.Split('.')[1]);
            return filePath;
        }
    }
}
