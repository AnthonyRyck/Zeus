using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorZeus.Areas.Identity;
using BlazorZeus.Data;
using BlazorZeus.Codes;
using BlazorZeus.Codes.Wish;
using System.IO;

namespace BlazorZeus
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")),
				ServiceLifetime.Singleton, ServiceLifetime.Singleton);

			// Au cas ou je repasse sur SQLServer
			//services.AddDbContext<ApplicationDbContext>(options =>
			//			options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")),
			//			ServiceLifetime.Singleton, ServiceLifetime.Singleton);

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddRazorPages();
			services.AddServerSideBlazor();

			services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

			// *** Service pour l'application ***
			services.AddSingleton<ISettings, SettingsManager>();
			services.AddSingleton<IMailing, MailingService>();

			services.AddSingleton<IMovies, MoviesManager>();
			services.AddSingleton<IShows, ShowsManager>();

			services.AddSingleton<IWish, WishMaster>();

			services.AddScoped<ITheMovieDatabase, MovieDatabase>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Création de la base, des roles et de Root.
			DataInitializer.CreateDatabase(app.ApplicationServices).Wait();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}




	

	public static class DataInitializer
	{
		private static readonly string[] Roles = new string[] { "Admin", "Manager", "Member" };

		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				foreach (var role in Roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}

				// Création de l'utilisateur Root.
				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
				var user = await userManager.FindByNameAsync("root");

				if (user == null)
				{
					var poweruser = new IdentityUser
					{
						UserName = "root",
						Email = "change@email.com",
						EmailConfirmed = true
					};
					string userPwd = "Azerty123!";

					var createPowerUser = await userManager.CreateAsync(poweruser, userPwd);
					if (createPowerUser.Succeeded)
					{
						await userManager.AddToRoleAsync(poweruser, "Admin");
					}
				}
			}
		}


		public static async Task CreateDatabase(IServiceProvider serviceProvider)
		{
			// Tester la présence de la db
			string pathDirectory = Path.Combine(AppContext.BaseDirectory, "Database");
			string pathFileDb = Path.Combine(pathDirectory, "appBlazor.db");

			if (!Directory.Exists(pathDirectory))
				Directory.CreateDirectory(pathDirectory);

			if (!File.Exists(pathFileDb))
			{
				// Créer la DB.
				using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
				{
					var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					context.Database.EnsureCreated();
				}

				await SeedRolesAsync(serviceProvider);
			}
		}
	}


}
