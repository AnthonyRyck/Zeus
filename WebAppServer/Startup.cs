using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAppServer.Codes;
using WepAppServer.Data;
using WepAppServer.Services;

namespace WebAppServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddDbContext<ApplicationDbContext>(options =>
		        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")),
				ServiceLifetime.Singleton, ServiceLifetime.Singleton);

	        services.AddIdentity<ApplicationUser, IdentityRole>()
		        .AddEntityFrameworkStores<ApplicationDbContext>()
		        .AddDefaultTokenProviders();

	        services.AddMvc()
		        .AddRazorPagesOptions(options =>
		        {
			        options.Conventions.AuthorizeFolder("/Account/Manage");
					options.Conventions.AuthorizePage("/Account/Logout");
			        options.Conventions.AuthorizeFolder("/Videos");
				});

			services.AddSingleton<ISettings, SettingsManager>();
			services.AddSingleton<IMailing, MailingService>();
			
			services.AddSingleton<IMovies, MoviesManager>();
			services.AddSingleton<IShows, ShowsManager>();

			// Register no-op EmailSender used by account confirmation and password reset during development
			// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
			services.AddSingleton<IEmailSender, EmailSender>();

			services.AddScoped<IVideoStreamService, VideoStreamService>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

			await CreateRoles(serviceProvider);
			
			app.UseAuthentication();

			app.UseMvc();
        }

		#region Private methods

		private async Task CreateRoles(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			
			string[] roleNames = { "Admin", "Manager", "Member" };

			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Création de l'utilisateur Root.
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var user = await userManager.FindByEmailAsync("change@email.com");

			if (user == null)
			{
				var poweruser = new ApplicationUser
				{
					UserName = "root",
					Email = "change@email.com",
				};
				string userPwd = "Azerty123!";

				var createPowerUser = await userManager.CreateAsync(poweruser, userPwd);
				if (createPowerUser.Succeeded)
				{
					await userManager.AddToRoleAsync(poweruser, "Admin");
				}
			}
		}

		#endregion

	}
}
