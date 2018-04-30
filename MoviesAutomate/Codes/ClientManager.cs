using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using WebAppServer.Models;

namespace MoviesAutomate.Codes
{
    public class ClientManager
    {
        #region Properties

        private readonly MovieManager _movieManager;
        private readonly TMDbClient _clientTmDb;
        private readonly StorageManager _storage;
        private readonly ConfigAppClient _configurationApp;
        private static readonly ILog _logger
            = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contient des informations de TmDB.
        /// </summary>
        private List<MovieModel> _movieModelsCollection = null;

        private Timer _timerUpdateMovieServer;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        private static readonly Object Lock = new object();

        /// <summary>
        /// Objet permettant d'aller taper sur notre serveur.
        /// </summary>
        private readonly MoviesServer _moviesServer;

        /// <summary>
        /// Indicateur si l'application est entrain de mettre à jour
        /// ces informations de films.
        /// </summary>
        private static bool _isUpdateMovies;

        #endregion

        #region Constructeur

        public ClientManager()
        {
            // TODO : Mettre en paramètre pour que ce soit configurable.
            _movieManager = new MovieManager("FRENCH", "TRUEFRENCH", "FR");
            _clientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

            _storage = new StorageManager();
            _configurationApp = _storage.GetConfiguration();
            _moviesServer = new MoviesServer(_configurationApp.UrlServer);

            var tempCollectionMovieModels = _storage.GetMoviesTmDb();
            if (tempCollectionMovieModels != null)
                _movieModelsCollection = tempCollectionMovieModels.ToList();

            _timerUpdateMovieServer = new Timer(TimerUpdateServerMovies, null, 15000, _configurationApp.TempsEnMillisecondPourTimerRefresh);
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
                await Task.Delay(1000);

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

                Movie movieDb;

                if (movieSelected == null)
                {
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
        /// Méthode pour rechercher dans la liste des résultats, le "bon" film.
        /// </summary>
        /// <param name="allMovies"></param>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retourne l'endroit ou sauvegarde le film.
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
        private string GetPathToSaveMovies(MovieInformation movieInformation)
        {
            return GetPathCore(movieInformation, _configurationApp.PathMovies);
        }

        /// <summary>
        /// Retourne l'endroit ou sauvegarde le dessin animé.
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
        private string GetPathToSaveDessinAnimes(MovieInformation movieInformation)
        {
            return GetPathCore(movieInformation, _configurationApp.PathDessinAnimes);
        }

        /// <summary>
        /// Méthode Core pour le choix du chemin pour sauvegarde d'une vidéo.
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <param name="pathPossible"></param>
        /// <returns></returns>
        private string GetPathCore(MovieInformation movieInformation, IEnumerable<string> pathPossible)
        {
            // En fonction de la taille du fichier il faut trouver un endroit ou le stocker.
            string emplacement = String.Empty;

            foreach (string path in pathPossible)
            {
                // Récupération de la lettre du Drive
                string drive = path[0].ToString();
                DriveInfo di = new DriveInfo(drive);

                if (di.AvailableFreeSpace > movieInformation.Size)
                {
                    // TODO : Lever une exception quand pas assez de place sur aucun lecteur.
                    emplacement = path;
                    break;
                }
            }

            return emplacement;
        }

        /// <summary>
        /// Méthode permettant de mettre à jour les films en local.
        /// </summary>
        private async Task UpdateLocalMovies()
        {
            // Récupération des films en locale.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                if (Directory.Exists(pathMovie))
                {
                    IEnumerable<MovieInformation> tempMoviesOnLocal = _movieManager.GetMoviesInformations(pathMovie, TypeVideo.Movie);

                    if (tempMoviesOnLocal.Any())
                        videosOnLocal.AddRange(tempMoviesOnLocal);
                }
                else
                {
                    _logger.Warn("Le répertoire Films n'existe pas  !!! - " + pathMovie);
                }
            }

            foreach (var pathDessinAnimes in _configurationApp.PathDessinAnimes)
            {
                if (Directory.Exists(pathDessinAnimes))
                {
                    IEnumerable<MovieInformation> tempDessinAnimes = _movieManager.GetMoviesInformations(pathDessinAnimes, TypeVideo.DessinAnime);

                    if (tempDessinAnimes.Any())
                        videosOnLocal.AddRange(tempDessinAnimes);
                }
                else
                {
                    _logger.Warn("Le répertoire DessinAnimes n'existe pas  !!! - " + pathDessinAnimes);
                }
            }

            List<MovieModel> listeToDelete = new List<MovieModel>();

            // Détermination des différences entre ce qui est présent sur le disque
            // et ce qui est connu en mémoire.
            if (_movieModelsCollection == null)
            {
                _movieModelsCollection = new List<MovieModel>();
            }

            foreach (MovieModel movieLocal in _movieModelsCollection)
            {
                if (!videosOnLocal.Contains(movieLocal.MovieInformation))
                {
                    listeToDelete.Add(movieLocal);
                }
            }

            lock (Lock)
            {
                // Suppression des films n'existant plus
                foreach (var toDelete in listeToDelete)
                {
                    _logger.Info("Suppression de la video en mémoire - " + toDelete.MovieInformation.Titre);
                    _movieModelsCollection.Remove(toDelete);
                }
            }

            // Voir s'il y a des rajouts.
            List<MovieInformation> tempMovieInformations = _movieModelsCollection.Select(x => x.MovieInformation).ToList();
            List<MovieInformation> listeToAdd = new List<MovieInformation>();

            foreach (MovieInformation movieLocal in videosOnLocal)
            {
                if (!tempMovieInformations.Contains(movieLocal))
                {
                    _logger.Info("Ajout de la video en mémoire - " + movieLocal.Titre);
                    listeToAdd.Add(movieLocal);
                }
            }

            var tempAddMovieModels = await GetMovieDbInformation(listeToAdd);
            lock (Lock)
            {
                _movieModelsCollection.AddRange(tempAddMovieModels);
            }

            // Sauvegarde
            _logger.Info("Sauvegarde Films et DessinAnimes : " + _movieModelsCollection.Count + " éléments connus");
            _storage.SaveMoviesModels(_movieModelsCollection);
        }

        /// <summary>
        /// Récupération des Films sur le Serveur.
        /// </summary>
        /// <returns></returns>
        private async Task GetMoviesOnServer()
        {
            IEnumerable<MovieInformation> moviesInformations = await _moviesServer.GetMoviesInformationAsync();

            List<MovieInformation> tempLocalMovie = _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.Movie)
                                                                        .Select(x => x.MovieInformation)
                                                                        .ToList();
            List<MovieInformation> listNewMovies = new List<MovieInformation>();

            foreach (MovieInformation movieInformation in moviesInformations)
            {
                if (!tempLocalMovie.Contains(movieInformation))
                {
                    _logger.Info("Ajout du film - " + movieInformation.Titre);
                    listNewMovies.Add(movieInformation);
                }
            }

            // Récupérer chaque nouveau film.
            foreach (MovieInformation newMovie in listNewMovies)
            {
                _moviesServer.DownloadVideo(newMovie, GetPathToSaveMovies);
            }
        }

        /// <summary>
        /// Récupération des dessins animes sur le Serveur.
        /// </summary>
        /// <returns></returns>
        private async Task GetDessinAnimesOnServer()
        {
            IEnumerable<MovieInformation> dessinAnimesInformation = await _moviesServer.GetDessinsAnimesInformationAsync();

            List<MovieInformation> tempLocalDessinAnimes = _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.DessinAnime)
                                                                          .Select(x => x.MovieInformation)
                                                                          .ToList();
            List<MovieInformation> listNewDessinAnimes = new List<MovieInformation>();

            foreach (MovieInformation movieInformation in dessinAnimesInformation)
            {
                if (!tempLocalDessinAnimes.Contains(movieInformation))
                {
                    _logger.Info("Ajout du dessin animes - " + movieInformation.Titre);
                    listNewDessinAnimes.Add(movieInformation);
                }
            }

            // Récupérer chaque nouveau dessin animes.
            foreach (MovieInformation newMovie in listNewDessinAnimes)
            {
                _moviesServer.DownloadVideo(newMovie, GetPathToSaveDessinAnimes);
            }
        }

        #endregion

        #region Timer Methods

        /// <summary>
        /// Ce Timer va chercher les informations sur les potentiels nouveaux
        /// films sur le serveur.
        /// </summary>
        /// <param name="state"></param>
        private async void TimerUpdateServerMovies(object state)
        {
            if (_isUpdateMovies)
            {
                _logger.Debug("_isUpdateMovie = TRUE - pas de mise à jour");
                return;
            }

            _isUpdateMovies = true;

            await UpdateLocalMovies();

            if (string.IsNullOrEmpty(_configurationApp.UrlServer))
            {
                _isUpdateMovies = false;
                _logger.Debug("Pas d'URL de server - Pas de synchronisation.");
                return;
            }

            await GetMoviesOnServer();
            await GetDessinAnimesOnServer();

            _isUpdateMovies = false;
        }

        #endregion
    }
}
