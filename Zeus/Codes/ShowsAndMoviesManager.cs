using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Client;

namespace Zeus.Codes
{
    /// <summary>
    /// Classe qui va faire la gestion des acquisitions des séries
    /// et des films.
    /// </summary>
    public class ShowsAndMoviesManager
    {
        #region Properties

        private MovieManager _movieManager;
        private TMDbClient _clientTmDb;
        private StorageManager _storage;
        private ConfigurationApp _configurationApp;

        /// <summary>
        /// Chemin d'accès pour les films.
        /// </summary>
        private string _pathMoviesLocal;

        /// <summary>
        /// Chemin d'accès pour les séries.
        /// </summary>
        private string _pathShowsLocal;

        #endregion

        #region Constructeur

        public ShowsAndMoviesManager()
        {
            // TODO : Mettre en paramtère pour que ce soit configurable.
            _movieManager = new MovieManager("FRENCH", "TRUEFRENCH", "FR");
            _clientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

            _storage = new StorageManager(_movieManager.GetMoviesInformations);
            _configurationApp = _storage.GetConfiguration();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retourne la liste des films qui sont présent sur le local.
        /// </summary>
        public IEnumerable<MovieInformation> GetListMoviesLocal()
        {
            IEnumerable<MovieInformation> allMovies = _storage.GetMoviesOnLocal(_configurationApp.PathMovies);
            return allMovies;
        }

        #endregion

    }
}
