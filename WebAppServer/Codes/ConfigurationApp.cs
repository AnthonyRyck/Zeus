using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Codes
{
    public class ConfigurationApp
    {
        /// <summary>
        /// Liste des langues pour la récupération des films/séries
        /// </summary>
        public List<string> ListeDeLangue { get; set; }

        /// <summary>
        /// Langue par défault pour l'API TMDb (pour les descriptions,...)
        /// </summary>
        public string LanguePourTmDb { get; set; }

        /// <summary>
        /// Région par défault pour l'API TMDb;
        /// </summary>
        public string RegionPourTmDb { get; set; }

        /// <summary>
        /// Chemin d'accés pour les Films.
        /// </summary>
        public IEnumerable<string> PathMovies { get; set; }

        /// <summary>
        /// Chemin d'accès pour les Dessin animés.
        /// </summary>
        public IEnumerable<string> PathDessinAnimes { get; set; }

        /// <summary>
        /// Chemin d'accés pour les séries.
        /// </summary>
        public IEnumerable<string> PathShows { get; set; }

        /// <summary>
        /// Temps en milliseconde pour faire un update sur les films/séries.
        /// </summary>
        public int TempsEnMillisecondPourTimerRefresh { get; set; }
    }
}
