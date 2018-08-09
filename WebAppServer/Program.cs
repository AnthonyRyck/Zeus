using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

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

			Log.Logger = new LoggerConfiguration()
		        .MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.WriteTo.RollingFile(Path.Combine(pathLog, "log-{Date}.txt"))
				.CreateLogger();

			try
	        {
				Log.Information("***** Lancement de l'application *****");
		        BuildWebHost(args).Run();
			}
	        catch (Exception e)
	        {
				Log.Fatal(e, "Exception levé dans le Main");
				Log.CloseAndFlush();
			}
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
				.UseSerilog()
                .Build();
    }
}
