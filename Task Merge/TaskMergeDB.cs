using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Task_Merge
{
    public class TaskMergeDB : DbContext
    {
		public DbSet<customer> users { get; set; } 

		public TaskMergeDB()
		{
			Database.EnsureCreated();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=my_zone_db;Username=postgres;Password=12345");
		}
	}
}

public class customer
{
	[Key]
	public string id { get; set; }
	[MaxLength(50)]
	[NotNull]
	public string name { get; set; }
	public string surname { get; set; }
	[MaxLength(255)]
	public string phone { get; set; }
	[NotNull]
	public string password { get; set; }
	public string user_type { get; set; }
}
public class task
{
	[NotNull]
	public string content { get; set; }
	public string student_id { get; set; }
	public string teacher_id { get; set; }
	public bool status { get; set; }	
	public string mark { get; set; }
}



