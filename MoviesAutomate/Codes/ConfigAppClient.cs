using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAutomate.Codes
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
        public IEnumerable<string> PathMovies { get; set; }

        /// <summary>
        /// Chemin d'accés pour les séries.
        /// </summary>
        public string PathShows { get; set; }

        /// <summary>
        /// Temps en milliseconde pour faire un update sur les films/séries.
        /// </summary>
        public int TempsEnMillisecondPourTimerRefresh { get; set; }

        /// <summary>
        /// URL du server contenant les films.
        /// </summary>
        public string UrlServer { get; set; }
    }
}
