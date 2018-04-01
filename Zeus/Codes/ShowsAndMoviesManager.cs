using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using Zeus.Models;

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

        /// <summary>
        /// Contient des informations de TmDB.
        /// </summary>
        private List<MovieModel> _movieModelsCollection = null;

        /// <summary>
        /// Contient la liste des films présent en local.
        /// </summary>
        private List<MovieInformation> _moviesLocal = null;

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
            if (_moviesLocal == null)
            {
                _moviesLocal = _storage.GetMoviesOnLocal(_configurationApp.PathMovies).ToList();
            }
            
            return _moviesLocal;
        }

        /// <summary>
        /// Retourne la liste des films avec toutes les informations de chaque film.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieModel>> GetMovies()
        {
            if (_movieModelsCollection != null)
            {
                return _movieModelsCollection;
            }

            // Voir dans le fichier de sauvegarde.
            IEnumerable<MovieModel> tempMovieModels = _storage.GetMoviesTmDb();
            if (tempMovieModels != null)
            {
                _movieModelsCollection = tempMovieModels.ToList();
                return _movieModelsCollection;
            }

            // Dans le cas ou il n'y a pas de fichier de sauvegarde.
            IEnumerable<MovieInformation> tempMovieLocal = GetListMoviesLocal();
            _movieModelsCollection = new List<MovieModel>();

            foreach (MovieInformation movieInformation in tempMovieLocal)
            {
                SearchMovie movieSelected;

                if (movieInformation.Annee != "Inconnu")
                {
                    SearchContainer<SearchMovie> temp = await _clientTmDb.SearchMovieAsync(movieInformation.Titre,
                        includeAdult: true,
                        year: Convert.ToInt32(movieInformation.Annee));

                    movieSelected = GetTheGoodMovie(temp, movieInformation);
                }
                else
                {
                    SearchContainer<SearchMovie> temp = await _clientTmDb.SearchMovieAsync(movieInformation.Titre,
                        includeAdult: true);

                    movieSelected = GetTheGoodMovie(temp, movieInformation);
                }

                Movie movieDb = await _clientTmDb.GetMovieAsync(movieSelected.Id);
                
                _movieModelsCollection.Add(new MovieModel()
                {
                    MovieInformation = movieInformation,
                    MovieTmDb = movieDb
                });
            }

            _storage.SaveMoviesModels(_movieModelsCollection);
            return _movieModelsCollection;
        }

        #endregion

        #region Private Methods

        private SearchMovie GetTheGoodMovie(SearchContainer<SearchMovie> allMovies, MovieInformation movieInformation)
        {
            SearchMovie retourMovie = null;

            foreach (var movie in allMovies.Results)
            {
                if (movie.Title.ToUpper() == movieInformation.Titre 
                    || movie.OriginalTitle.ToUpper() == movieInformation.Titre)
                {
                    retourMovie = movie;
                    break;
                }
            }

            return retourMovie;
        }

        #endregion

    }
}
