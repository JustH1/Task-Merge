using Microsoft.AspNetCore.Authentication.Cookies;

namespace Task_Merge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            //��������� ������ ��� ������ � ����� ������.
            builder.Services.AddTransient<TaskMergeDB>();
			builder.Services.AddTransient<TaskMergeRole>();
            builder.Services.AddLogging();

			//���������� ������ ��� ������ � Cookie
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            var app = builder.Build();

            ////
            app.Environment.EnvironmentName = "Development";
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            ////

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapRazorPages();

            app.Run();
        }
    }
}