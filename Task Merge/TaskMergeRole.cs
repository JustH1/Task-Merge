using Microsoft.AspNetCore.Identity;

namespace Task_Merge
{
	public class TaskMergeRole
	{
		public IdentityRole student = new IdentityRole("student");
		public IdentityRole teacher = new IdentityRole("teacher");
	}
}
