using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorZeus.Codes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorZeus
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//CreateHostBuilder(args).Build().Run();

			var host = CreateHostBuilder(args).Build();

			// Crée un Scope le temps de pouvoir créer l'admin de base.
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				CreateRolesAndAdmin(services);
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});


		private static async Task CreateRolesAndAdmin(IServiceProvider serviceProvider)
		{
			// Création de l'utilisateur Root.
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			var user = await userManager.FindByEmailAsync("change@email.com");

			if (user == null)
			{
				var poweruser = new IdentityUser
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
	}
}
