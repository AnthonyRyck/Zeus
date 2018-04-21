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

namespace WebAppClient.Codes
{
    public class ClientManager : IClientManager
    {
        #region Properties

        private readonly MovieManager _movieManager;
        private readonly TMDbClient _clientTmDb;
        private readonly StorageManager _storage;
        private readonly ConfigAppClient _configurationApp;

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

            _storage = new StorageManager(_movieManager.GetMoviesInformations);
            _configurationApp = _storage.GetConfiguration();
            _moviesServer = new MoviesServer(_configurationApp.UrlServer, GetPathToSave);

            _timerUpdateMovieServer = new Timer(TimerUpdateServerMovies, null, 15000, _configurationApp.TempsEnMillisecondPourTimerRefresh);
        }

        #endregion

        #region Implement IClientManager

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

                if (tempMovieLocal.Any())
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

                if (movieSelected == null)
                {
                    // TODO : Mettre en log le fait qu'aucun film de trouvé.
                    // TODO : Faire un objet Movie "factice" pour juste l'affichage.
                    //Movie movieDb = new Movie();

                }
                else
                {
                    Movie movieDb = await _clientTmDb.GetMovieAsync(movieSelected.Id);

                    returnMovieModels.Add(new MovieModel(Guid.NewGuid())
                    {
                        MovieInformation = movieInformation,
                        MovieTmDb = movieDb
                    });
                }
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
        private string GetPathToSave(MovieInformation movieInformation)
        {
            // En fonction de la taille du fichier il faut trouver un endroit ou le stocker.
            string emplacement = String.Empty;

            foreach (string pathMovie in _configurationApp.PathMovies)
            {
                // Récupération de la lettre du Drive
                string drive = pathMovie[0].ToString();
                DriveInfo di = new DriveInfo(drive);

                if (di.AvailableFreeSpace > movieInformation.Size)
                {
                    // TODO : Lever une exception quand pas assez de place sur aucun lecteur.
                    emplacement = pathMovie;
                    break;
                }
            }

            return emplacement;
        }

        #endregion

        #region Timer Methods

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
                    // TODO : Mettre en log l'erreur que le repertoire n'existe pas.
                }
            }

            foreach (var pathAnime in _configurationApp.PathDessinAnimes)
            {
                if (Directory.Exists(pathAnime))
                {
                    IEnumerable<MovieInformation> tempDessinAnimesOnLocal = _movieManager.GetMoviesInformations(pathAnime, TypeVideo.DessinAnime);

                    if (tempDessinAnimesOnLocal.Any())
                        videosOnLocal.AddRange(tempDessinAnimesOnLocal);
                }
                else
                {
                    // TODO : Mettre en log l'erreur que le repertoire n'existe pas.
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
                    listeToAdd.Add(movieLocal);
                }
            }

            var tempAddMovieModels = await GetMovieDbInformation(listeToAdd);
            lock (Lock)
            {
                _movieModelsCollection.AddRange(tempAddMovieModels);
            }

            // Sauvegarde
            _storage.SaveMoviesModels(_movieModelsCollection);
        }

        /// <summary>
        /// Ce Timer va chercher les informations sur les potentiels nouveaux
        /// films sur le serveur.
        /// </summary>
        /// <param name="state"></param>
        private async void TimerUpdateServerMovies(object state)
        {
            if (_isUpdateMovies)
                return;

            _isUpdateMovies = true;

            await UpdateLocalMovies();

            if (string.IsNullOrEmpty(_configurationApp.UrlServer))
            {
                _isUpdateMovies = false;
                return;
            }

            IEnumerable<MovieInformation> moviesInformations = await _moviesServer.GetMoviesInformationAsync();

            List<MovieInformation> tempLocalMovie = _movieModelsCollection.Select(x => x.MovieInformation).ToList();
            List<MovieInformation> listNewMovies = new List<MovieInformation>();

            foreach (MovieInformation movieInformation in moviesInformations)
            {
                if (!tempLocalMovie.Contains(movieInformation))
                {
                    listNewMovies.Add(movieInformation);
                }
            }

            // Récupérer chaque nouveau film.
            foreach (MovieInformation newMovie in listNewMovies)
            {
                _moviesServer.DownloadMovies(newMovie);
            }

            _isUpdateMovies = false;
        }

        #endregion
    }
}
