using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebAppClient
{
    public class Program
    {

        #region ServiceOrConsole
        public static void Main(string[] args)
        {
            Console.WriteLine("***** Lancement de l'application *******");
            Console.WriteLine("Appuyez sur une touche pour commencer.");
            Console.ReadKey();

            //bool isService = true;
            //if (Debugger.IsAttached || args.Contains("--console"))
            //{
            //    isService = false;
            //}

            var pathToContentRoot = Directory.GetCurrentDirectory();
            //if (isService)
            //{
            //    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            //    pathToContentRoot = Path.GetDirectoryName(pathToExe);
            //}

            var host = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(pathToContentRoot)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            //if (isService)
            //{
            //    host.RunAsService();
            //}
            //else
            //{
                host.Run();
            //}
        }
        #endregion

    }
}
