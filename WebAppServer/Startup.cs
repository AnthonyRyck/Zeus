using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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
		        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

	        services.AddIdentity<ApplicationUser, IdentityRole>()
		        .AddEntityFrameworkStores<ApplicationDbContext>()
		        .AddDefaultTokenProviders();

	        services.AddMvc()
		        .AddRazorPagesOptions(options =>
		        {
			        options.Conventions.AuthorizeFolder("/Account/Manage");
			        options.Conventions.AuthorizePage("/Account/Logout");
		        });

			MoviesManager moviesManager = new MoviesManager();
            services.AddSingleton<IMovies>(moviesManager);

			ShowsManager showManagers = new ShowsManager();
	        services.AddSingleton<IShows>(showManagers);

	        // Register no-op EmailSender used by account confirmation and password reset during development
	        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
	        services.AddSingleton<IEmailSender, EmailSender>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            Directory.CreateDirectory(env.ContentRootPath + "/Logs");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath + "/Logs/", "log-{Date}.txt"))
                .CreateLogger();

            loggerFactory.AddSerilog();

            app.UseStaticFiles();

	        app.UseAuthentication();

			app.UseMvc();
        }

       
    }
}
