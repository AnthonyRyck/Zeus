﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using Newtonsoft.Json;
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

        private MovieManager _movieManager;
        private TMDbClient _clientTmDb;
        private StorageManager _storage;
        private ConfigAppClient _configurationApp;

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
        private Timer _timerUpdateMovieServer;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        private static readonly Object _lock = new object();

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        private static readonly Object _lockAddItem = new object();

        /// <summary>
        /// Objet permettant d'aller taper sur notre serveur.
        /// </summary>
        private readonly MoviesServer _moviesServer;

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

            _timerUpdate = new Timer(TimerUpdate, null, 5000, _configurationApp.TempsEnMillisecondPourTimerRefresh);
            _timerUpdateMovieServer = new Timer(TimerUpdateServerMovies, null, 15000, _configurationApp.TempsPourRefreshMovieServer);
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
        /// Méthode qui appelé lorsque le Timer arrive à la fin.
        /// </summary>
        /// <param name="state"></param>
        private async void TimerUpdate(object state)
        {
            // Récupération des films en locale.
            List<MovieInformation> moviesOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in _configurationApp.PathMovies)
            {
                IEnumerable<MovieInformation> tempMoviesOnLocal = _movieManager.GetMoviesInformations(pathMovie);

                if(tempMoviesOnLocal.Any())
                    moviesOnLocal.AddRange(tempMoviesOnLocal);
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

            // Voir s'il y a des rajouts.
            List<MovieInformation> tempMovieInformations =
                _movieModelsCollection.Select(x => x.MovieInformation).ToList();
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

        /// <summary>
        /// Ce Timer va chercher les informations sur les potentiels nouveaux
        /// films sur le serveur.
        /// </summary>
        /// <param name="state"></param>
        private async void TimerUpdateServerMovies(object state)
        {
            if (string.IsNullOrEmpty(_configurationApp.UrlServer))
                return;

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
                await _moviesServer.DownloadMovies(newMovie);
            }
        }

        #endregion
    }
}
