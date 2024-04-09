using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Task_Merge
{
    public class TaskMergeDB : DbContext
    {
		public DbSet<customer> customer { get; set; } 
		public DbSet<task> task { get; set; }

		public TaskMergeDB()
		{
			Database.EnsureCreated();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=digital_department_practice;Username=postgres;Password=12345");
		}
	}
}

public class customer
{
	[Key]
	public int id { get; set; }
	[MaxLength(50)]
	[NotNull]
	public string name { get; set; }
	[MaxLength(255)]
	public string email { get; set; }
	[NotNull]
	public string password { get; set; }
	public string user_type { get; set; }
}
public class task
{
	[NotNull]
	public string content { get; set; }
	public int student_id { get; set; }
	public int teacher_id { get; set; }
	public bool status { get; set; }	
	public int mark { get; set; }
}



