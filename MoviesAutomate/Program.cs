using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MoviesAutomate.Codes;

namespace MoviesAutomate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("######### Début de l'application #########");
            Console.ReadKey();

            ClientManager manager = new ClientManager();

            
            string exitMessage = string.Empty;
            while (exitMessage != "EXIT")
            {
                Console.WriteLine("######### Tapez EXIT pour quitter #########");
                exitMessage = Console.ReadLine();
            }

            Console.WriteLine("######### Fin de l'application #########");
            Console.ReadKey();
        }
    }
}
