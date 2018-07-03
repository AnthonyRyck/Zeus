using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebAppServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        string pathLog = AppContext.BaseDirectory + "/Logs/";
	        if (!Directory.Exists(pathLog))
	        {
		        Directory.CreateDirectory(pathLog);
	        }

			var logger = new LoggerConfiguration()
		        .MinimumLevel.Debug()
		        .WriteTo.File(Path.Combine(pathLog, "log-system.txt"))
		        .CreateLogger();

			try
	        {
				logger.Information("***** Lancement de l'application *****");
		        BuildWebHost(args).Run();
			}
	        catch (Exception e)
	        {
		        logger.Fatal(e, "Exception levé dans le Main");
			}
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
