using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MoviesAutomate.Codes;

namespace MoviesAutomate
{
    class Program
    {
        // On définit une variable logger static qui référence l'instance du logger nommé Program
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Console.WriteLine("######### Début de l'application #########");
            Console.ReadKey();

            log.Info("Début de l'application");

            ClientManager manager = new ClientManager();

            
            string exitMessage = string.Empty;
            while (exitMessage != "EXIT")
            {
                Console.WriteLine("######### Tapez EXIT pour quitter #########");
                exitMessage = Console.ReadLine();
            }

            log.Info("Fin de l'application");

            Console.WriteLine("######### Fin de l'application #########");
            Console.ReadKey();
        }
    }
}
