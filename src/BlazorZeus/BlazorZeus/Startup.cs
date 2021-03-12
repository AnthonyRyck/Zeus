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
using BlazorZeus.Services;
using BlazorDownloadFile;
using System.Net.Http;
using BlazorZeus.Composants.ViewModel;
using BlazorZeus.ViewModel;

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

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddRazorPages();
			services.AddServerSideBlazor();

			services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

			// *** Service pour l'application ***
			services.AddSingleton<ISettings, SettingsManager>();
			services.AddSingleton<IMovies, MoviesManager>();
			
			//services.AddSingleton<IShows, ShowsManager>();
			//services.AddSingleton<IWish, WishMaster>();

			services.AddScoped<ITheMovieDatabase, MovieDatabase>();
			services.AddScoped<IGestionLog, GestionLog>();
			services.AddScoped<IMoviesViewModel, MoviesViewModel>();
			services.AddScoped<IUsersViewModel, UsersViewModel>();
			services.AddScoped<IMovieViewModel, MovieViewModel>();

			services.AddHostedService<AnalyserHostedService>();
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
}
