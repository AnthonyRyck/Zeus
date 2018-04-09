using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppClient.Codes
{
    public class ConfigAppClient
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

        /// <summary>
        /// Temps en milliseconde pour faire un update sur les films/séries.
        /// </summary>
        public int TempsEnMillisecondPourTimerRefresh { get; set; }

        /// <summary>
        /// Temps en milliseconde pour faire une nouvelle recherche sur le server
        /// s'il y a des nouveaux films.
        /// </summary>
        public int TempsPourRefreshMovieServer { get; set; }

        /// <summary>
        /// URL du server contenant les films.
        /// </summary>
        public string UrlServer { get; set; }
    }
}
