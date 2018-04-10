using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    /// <summary>
    /// Classe qui va faire la gestion des acquisitions des séries
    /// et des films.
    /// </summary>
    public class ShowsAndMoviesManager : IShowsAndMovies
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

        private Timer _timerUpdate;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        private static readonly Object _lock = new object();

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

            // Démarre dans 5secondes et toutes les 15 minutes.
            _timerUpdate = new Timer(TimerUpdate, null, 5000, _configurationApp.TempsEnMillisecondPourTimerRefresh);
        }

        #endregion

        #region Public Methods - Implements IShowsAndMovies

        /// <summary>
        /// Retourne la liste des films qui sont présent sur le local.
        /// </summary>
        public async Task<IEnumerable<MovieInformation>> GetListMoviesLocal()
        {
            var temp = await GetMovies();
            return temp.Select(x => x.MovieInformation).ToList();
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
            List<MovieInformation> moviesOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                IEnumerable<MovieInformation> tempMovieLocal = _movieManager.GetMoviesInformations(pathMovie);

                if(tempMovieLocal.Any())
                    moviesOnLocal.AddRange(tempMovieLocal);
            }

            _movieModelsCollection = await GetMovieDbInformation(moviesOnLocal);
            _storage.SaveMoviesModels(_movieModelsCollection);

            return _movieModelsCollection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Méthode permettant d'aller chercher sur l'API de TmDb les informations sur les films.
        /// </summary>
        /// <param name="moviesInfomration"></param>
        /// <returns></returns>
        private async Task<List<MovieModel>> GetMovieDbInformation(IEnumerable<MovieInformation> moviesInfomration)
        {
            List<MovieModel> returnMovieModels = new List<MovieModel>();

            foreach (MovieInformation movieInformation in moviesInfomration)
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

                returnMovieModels.Add(new MovieModel()
                {
                    MovieInformation = movieInformation,
                    MovieTmDb = movieDb
                });
            }

            return returnMovieModels;
        }

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

        #region Timer Methods

        /// <summary>
        /// Méthode qui appelé lorsque le Timer arrive à la fin.
        /// </summary>
        /// <param name="state"></param>
        private async void TimerUpdate(object state)
        {
            // Récupération des films en locale.
            List<MovieInformation> moviesOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                if (!Directory.Exists(pathMovie))
                {
                    continue;
                }

                IEnumerable<MovieInformation> tempMoviesOnLocal = _movieManager.GetMoviesInformations(pathMovie);

                if (tempMoviesOnLocal.Any())
                    moviesOnLocal.AddRange(tempMoviesOnLocal);
            }

            List<MovieModel> listeToDelete = new List<MovieModel>();

            if (_movieModelsCollection != null)
            {
                // Détermination des différences entre ce qui est présent sur le disque
                // et ce qui est connu en mémoire.
                foreach (MovieModel movieLocal in _movieModelsCollection)
                {
                    if (!moviesOnLocal.Contains(movieLocal.MovieInformation))
                    {
                        listeToDelete.Add(movieLocal);
                    }
                }

                lock (_lock)
                {
                    // Suppression des films n'existant plus
                    foreach (var toDelete in listeToDelete)
                    {
                        _movieModelsCollection.Remove(toDelete);
                    }
                }
            }

            if(_movieModelsCollection == null)
                _movieModelsCollection = new List<MovieModel>();

           // Voir s'il y a des rajouts.
            List<MovieInformation> tempMovieInformations = _movieModelsCollection.Select(x => x.MovieInformation).ToList();
            List<MovieInformation> listeToAdd = new List<MovieInformation>();

            foreach (MovieInformation movieLocal in moviesOnLocal)
            {
                if (!tempMovieInformations.Contains(movieLocal))
                {
                    listeToAdd.Add(movieLocal);
                }
            }
            
            var tempAddMovieModels = await GetMovieDbInformation(listeToAdd);
            lock (_lock)
            {
                _movieModelsCollection.AddRange(tempAddMovieModels);
            }

            // Sauvegarde
            _storage.SaveMoviesModels(_movieModelsCollection);
        }

        #endregion

    }
}
