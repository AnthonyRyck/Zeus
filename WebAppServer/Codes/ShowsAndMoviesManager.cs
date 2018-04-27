using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        //private readonly ILogger _logger;

        /// <summary>
        /// Contient des informations de TmDB.
        /// </summary>
        private List<MovieModel> _movieModelsCollection = null;

        private Timer _timerUpdate;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        private static readonly Object _lock = new object();

        private static bool _isUpdateTime = false;

        #endregion

        #region Constructeur

        public ShowsAndMoviesManager()
        {
            //_logger = logger;

            // TODO : Mettre en paramtère pour que ce soit configurable.
            _movieManager = new MovieManager("FRENCH", "TRUEFRENCH", "FR");
            _clientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

            _storage = new StorageManager(_movieManager.GetMoviesInformations);
            _configurationApp = _storage.GetConfiguration();
            var tempCollectionMovieModels = _storage.GetMoviesTmDb();
            if (tempCollectionMovieModels != null)
                _movieModelsCollection = tempCollectionMovieModels.ToList();

            // Démarre dans 5 secondes et toutes les 15 minutes.
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
            return temp.Where(x => !x.IsDownloaded
                            && x.MovieInformation.TypeVideo == TypeVideo.Movie)
                       .Select(x => x.MovieInformation).ToList();
        }

        /// <summary>
        /// Retourne la liste des films qui sont présent sur le local.
        /// </summary>
        public async Task<IEnumerable<MovieInformation>> GetListDessinAnimesLocal()
        {
            var temp = await GetDessinAnimes();
            return temp.Where(x => !x.IsDownloaded
                                   && x.MovieInformation.TypeVideo == TypeVideo.DessinAnime)
                .Select(x => x.MovieInformation).ToList();
        }
        
        /// <summary>
        /// Retourne la liste des films avec toutes les informations de chaque film.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieModel>> GetMovies()
        {
            if (_movieModelsCollection != null)
            {
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.Movie).ToList();
            }

            // Voir dans le fichier de sauvegarde.
            IEnumerable<MovieModel> tempMovieModels = _storage.GetMoviesTmDb();
            if (tempMovieModels != null)
            {
                _movieModelsCollection = tempMovieModels.ToList();
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.Movie).ToList();
            }

            // Dans le cas ou il n'y a pas de fichier de sauvegarde.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();

            // Récupération des films.
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                IEnumerable<MovieInformation> tempMovieLocal = _movieManager.GetMoviesInformations(pathMovie, TypeVideo.Movie);

                if(tempMovieLocal.Any())
                    videosOnLocal.AddRange(tempMovieLocal);
            }
            
            _movieModelsCollection = await GetMovieDbInformation(videosOnLocal);
            _storage.SaveMoviesModels(_movieModelsCollection);

            return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.Movie).ToList();
        }

        /// <summary>
        /// Retourne la liste des films avec toutes les informations de chaque film.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieModel>> GetDessinAnimes()
        {
            if (_movieModelsCollection != null)
            {
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.DessinAnime).ToList();
            }

            // Voir dans le fichier de sauvegarde.
            IEnumerable<MovieModel> tempMovieModels = _storage.GetMoviesTmDb();
            if (tempMovieModels != null)
            {
                _movieModelsCollection = tempMovieModels.ToList();
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.DessinAnime).ToList();
            }

            // Dans le cas ou il n'y a pas de fichier de sauvegarde.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();

            // Récupération des dessins animés.
            foreach (var dessinAnimes in _configurationApp.PathDessinAnimes)
            {
                IEnumerable<MovieInformation> tempAnimes = _movieManager.GetMoviesInformations(dessinAnimes, TypeVideo.DessinAnime);

                if (tempAnimes.Any())
                    videosOnLocal.AddRange(tempAnimes);
            }

            _movieModelsCollection = await GetMovieDbInformation(videosOnLocal);
            _storage.SaveMoviesModels(_movieModelsCollection);

            return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.DessinAnime).ToList();
        }

        /// <summary>
        /// Indication qu'il faut mettre à jour ce film, car il a été téléchargé.
        /// </summary>
        /// <param name="movieInformation"></param>
        public void SetMovieDownloaded(MovieInformation movieInformation)
        {
            MovieModel movieDownloaded = _movieModelsCollection.FirstOrDefault(x => !x.IsDownloaded
                                                                      && x.MovieInformation.Titre == movieInformation.Titre
                                                                      && x.MovieInformation.FileName == movieInformation.FileName);

            if (movieDownloaded == null)
            {
                // TODO : Mettre un log sur une erreur produite.
            }
            else
            {
                movieDownloaded.IsDownloaded = true;
            }
        }

        /// <summary>
        /// Récupère la vidéo par rapport à l'ID donné.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MovieModel GetMovie(Guid id)
        {
            return _movieModelsCollection.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titre"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SearchVideoModel>> GetListVideoOnTmDb(string titre)
        {
            List<SearchVideoModel> retourInfo = new List<SearchVideoModel>();

            var temp = await _clientTmDb.SearchMovieAsync(titre);

            foreach (SearchMovie result in temp.Results)
            {
                if (!string.IsNullOrEmpty(result.PosterPath))
                {
                    retourInfo.Add(new SearchVideoModel()
                    {
                        UrlAffiche = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + result.PosterPath,
                        IdVideoTmDb = result.Id
                    });
                }
            }

            return retourInfo;
        }


        public async Task<MovieModel> ChangeVideo(Guid id, int idVideoTmDb)
        {
            MovieModel videoToChange = GetMovie(id);
            if (videoToChange == null)
                return null;

            Movie videoTmDb = await _clientTmDb.GetMovieAsync(idVideoTmDb);
            
            lock (_lock)
            {
                videoToChange.MovieTmDb = videoTmDb;
                _storage.SaveMoviesModels(_movieModelsCollection);
            }
            
            return videoToChange;
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

                // Pour palier le limit rate du site j'attend 1 sec avant de refaire une recherche.
                await Task.Delay(1000);

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

                Movie movieDb;

                if (movieSelected == null)
                {
                    //_logger.LogInformation("Aucune information TmDb trouvé pour " + movieInformation.Titre);

                    movieDb = new Movie
                    {
                        Title = movieInformation.Titre,
                        Overview = "Aucune information de trouvé sur TmDb"
                    };
                }
                else
                {
                    movieDb = await _clientTmDb.GetMovieAsync(movieSelected.Id);
                }

                returnMovieModels.Add(new MovieModel(Guid.NewGuid())
                {
                    MovieInformation = movieInformation,
                    MovieTmDb = movieDb
                });
            }

            return returnMovieModels;
        }

        /// <summary>
        /// Permet de faire la sélection de la "meilleur" vidéo.
        /// </summary>
        /// <param name="allMovies">Liste de résultat de video</param>
        /// <param name="movieInformation">Film recherché.</param>
        /// <returns></returns>
        private SearchMovie GetTheGoodMovie(SearchContainer<SearchMovie> allMovies, MovieInformation movieInformation)
        {
            SearchMovie retourMovie = null;

            if (allMovies.Results.Count == 1)
                return allMovies.Results[0];

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
            if (_isUpdateTime)
                return;

            _isUpdateTime = true;

            // Récupération des films en locale.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                if (!Directory.Exists(pathMovie))
                {
                    continue;
                }

                IEnumerable<MovieInformation> tempMoviesOnLocal = _movieManager.GetMoviesInformations(pathMovie, TypeVideo.Movie);

                if (tempMoviesOnLocal.Any())
                    videosOnLocal.AddRange(tempMoviesOnLocal);
            }

            // Récupération des dessins animés.
            foreach (var dessinAnimes in _configurationApp.PathDessinAnimes)
            {
                if (!Directory.Exists(dessinAnimes))
                {
                    continue;
                }

                IEnumerable<MovieInformation> tempAnimes = _movieManager.GetMoviesInformations(dessinAnimes, TypeVideo.DessinAnime);

                if (tempAnimes.Any())
                    videosOnLocal.AddRange(tempAnimes);
            }

            List<MovieModel> listeToDelete = new List<MovieModel>();

            if (_movieModelsCollection != null)
            {
                // Détermination des différences entre ce qui est présent sur le disque
                // et ce qui est connu en mémoire.
                foreach (MovieModel movieLocal in _movieModelsCollection)
                {
                    if (!videosOnLocal.Contains(movieLocal.MovieInformation))
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

            foreach (MovieInformation movieLocal in videosOnLocal)
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
            _isUpdateTime = false;
        }

        #endregion

    }
}
