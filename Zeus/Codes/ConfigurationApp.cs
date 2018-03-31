using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zeus.Codes
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
        public string PathMovies { get; set; }

        /// <summary>
        /// Chemin d'accés pour les séries.
        /// </summary>
        public string PathShows { get; set; }
    }
}
