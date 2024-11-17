using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.Extensions.FileProviders;

namespace ClinicManagementSystem
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Configure DbContext
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
	        options.UseSqlServer(connectionString));


			builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();



			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
				Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
				RequestPath = "/vendor"
			});

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			// Add role seeding
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				string[] roles = ["Admin", "User"];

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole<Guid>(role));
					}
				}

				// Optionally create an admin user
				var adminEmail = "admin@admin.com";
				var adminUser = await userManager.FindByEmailAsync(adminEmail);

				if (adminUser == null)
				{
					adminUser = new ApplicationUser
					{
						UserName = adminEmail,
						Email = adminEmail,
						FirstName = "Admin",
						LastName = "User",
						EmailConfirmed = true
					};

					var result = await userManager.CreateAsync(adminUser, "Admin@123");
					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(adminUser, "Admin");
					}
				}
			}


			app.Run();
		}
	}
}
