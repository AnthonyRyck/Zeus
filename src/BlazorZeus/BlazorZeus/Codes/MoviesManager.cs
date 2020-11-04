using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using BlazorZeus.Models;

namespace BlazorZeus.Codes
{
    /// <summary>
    /// Classe qui va faire la gestion des acquisitions des séries
    /// et des films.
    /// </summary>
    public class MoviesManager : AbstractManager, IMovies
    {
        #region Properties

        private readonly MovieManager _movieManager;

        /// <summary>
        /// Contient des informations de TmDB.
        /// </summary>
        private List<MovieModel> _movieModelsCollection = null;
        
        #endregion

        #region Constructeur

        public MoviesManager(ISettings settings)
		 : base(settings)
        {
            //_logger = logger;
	        IEnumerable<string> langues = settings.GetLanguesVideos();
            _movieManager = new MovieManager(langues.ToArray());
			Storage.SetFunc(_movieManager.GetMoviesInformations);

			var tempCollectionMovieModels = Storage.GetMoviesTmDb();
            if (tempCollectionMovieModels != null)
                _movieModelsCollection = tempCollectionMovieModels.ToList();
        }
        
        #endregion

        #region Public Methods - Implements IMovies

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
            IEnumerable<MovieModel> tempMovieModels = Storage.GetMoviesTmDb();
            if (tempMovieModels != null)
            {
                _movieModelsCollection = tempMovieModels.ToList();
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.Movie).ToList();
            }

            // Dans le cas ou il n'y a pas de fichier de sauvegarde.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();

            // Récupération des films.
            foreach (var pathMovie in Settings.GetPathMovies())
            {
                IEnumerable<MovieInformation> tempMovieLocal = _movieManager.GetMoviesInformations(pathMovie, TypeVideo.Movie);

                if(tempMovieLocal.Any())
                    videosOnLocal.AddRange(tempMovieLocal);
            }
            
            _movieModelsCollection = await GetMovieDbInformation(videosOnLocal);
            Storage.SaveMoviesModels(_movieModelsCollection);

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
            IEnumerable<MovieModel> tempMovieModels = Storage.GetMoviesTmDb();
            if (tempMovieModels != null)
            {
                _movieModelsCollection = tempMovieModels.ToList();
                return _movieModelsCollection.Where(x => x.MovieInformation.TypeVideo == TypeVideo.DessinAnime).ToList();
            }

            // Dans le cas ou il n'y a pas de fichier de sauvegarde.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();

            // Récupération des dessins animés.
            foreach (var dessinAnimes in Settings.GetPathDessinAnimes())
            {
                IEnumerable<MovieInformation> tempAnimes = _movieManager.GetMoviesInformations(dessinAnimes, TypeVideo.DessinAnime);

                if (tempAnimes.Any())
                    videosOnLocal.AddRange(tempAnimes);
            }

            _movieModelsCollection = await GetMovieDbInformation(videosOnLocal);
            Storage.SaveMoviesModels(_movieModelsCollection);

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
        /// Récupère du site TmDb les informations par rapport à un titre.
        /// </summary>
        /// <param name="titre"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SearchVideoModel>> GetListVideoOnTmDb(string titre)
        {
            List<SearchVideoModel> retourInfo = new List<SearchVideoModel>();

            var temp = await ClientTmDb.SearchMovieAsync(titre);

            foreach (SearchMovie result in temp.Results)
            {
                if (!string.IsNullOrEmpty(result.PosterPath))
                {
                    retourInfo.Add(new SearchVideoModel()
                    {
                        UrlAffiche = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + result.PosterPath,
                        IdVideoTmDb = result.Id,
						Titre = result.Title,
                        ReleaseDate = result.ReleaseDate
                    });
                }
            }

            return retourInfo;
        }

		/// <summary>
		/// Permet de change la video par rapport à son ID, en donnant
		/// l'ID de TheMovieDatabase.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="idVideoTmDb"></param>
		/// <returns></returns>
        public async Task<MovieModel> ChangeVideo(Guid id, int idVideoTmDb)
        {
            MovieModel videoToChange = GetMovie(id);
            if (videoToChange == null)
                return null;

            Movie videoTmDb = await ClientTmDb.GetMovieAsync(idVideoTmDb);
            
            lock (Lock)
            {
                videoToChange.MovieTmDb = videoTmDb;
                Storage.SaveMoviesModels(_movieModelsCollection);
            }
            
            return videoToChange;
        }

        public void ChangeResolution(Guid id, string quality)
        {
            MovieModel videoToChange = GetMovie(id);
            if (videoToChange == null)
                return;
                        
            lock (Lock)
            {
                videoToChange.MovieInformation.Resolution = quality;
                Storage.SaveMoviesModels(_movieModelsCollection);
            }
        }

        /// <summary>
        /// Permet de supprimer la video par rapport à son ID.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveVideo(Guid id)
	    {
		    lock (Lock)
		    {
			    MovieModel model = _movieModelsCollection.FirstOrDefault(x => x.Id == id);

			    if (model != null)
			    {
				    _movieModelsCollection.Remove(model);
				    Storage.SaveMoviesModels(_movieModelsCollection);
				}
			}
	    }


        public async Task<IEnumerable<MovieModel>> AnalysePaths()
		{
            // Récupération des films en locale.
            List<MovieInformation> videosOnLocal = new List<MovieInformation>();
            foreach (var pathMovie in Settings.GetPathMovies())
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
            foreach (var dessinAnimes in Settings.GetPathDessinAnimes())
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

                lock (Lock)
                {
                    // Suppression des films n'existant plus
                    foreach (var toDelete in listeToDelete)
                    {
                        _movieModelsCollection.Remove(toDelete);
                    }
                }
            }

            if (_movieModelsCollection == null)
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

            List<MovieModel> tempAddMovieModels = null;

            if (listeToAdd.Count > 0)
            {
				try
				{
                    tempAddMovieModels = await GetMovieDbInformation(listeToAdd);

                    lock (Lock)
                    {
                        _movieModelsCollection.AddRange(tempAddMovieModels);
                    }

                    // Sauvegarde
                    Storage.SaveMoviesModels(_movieModelsCollection);
                }
				catch (Exception ex)
				{

					throw;
				}
            }

            return tempAddMovieModels;
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
                    SearchContainer<SearchMovie> temp = await ClientTmDb.SearchMovieAsync(movieInformation.Titre,
                        includeAdult: true,
                        year: Convert.ToInt32(movieInformation.Annee));

                    movieSelected = GetTheGoodMovie(temp, movieInformation);
                }
                else
                {
                    SearchContainer<SearchMovie> temp = await ClientTmDb.SearchMovieAsync(movieInformation.Titre,
                        includeAdult: true);

                    movieSelected = GetTheGoodMovie(temp, movieInformation);
                }

                Movie movieDb;

                if (movieSelected == null)
                {
                    //Log.Information("Aucune information TmDb trouvé pour " + movieInformation.Titre);

                    movieDb = new Movie
                    {
                        Title = movieInformation.Titre,
                        Overview = "Aucune information de trouvé sur TmDb"
                    };
                }
                else
                {
                    movieDb = await ClientTmDb.GetMovieAsync(movieSelected.Id);
                }

                returnMovieModels.Add(new MovieModel(Guid.NewGuid())
                {
                    MovieInformation = movieInformation,
                    MovieTmDb = movieDb,
					DateAdded = GetDateFileCreated(movieInformation.PathFile)
                });
            }

            return returnMovieModels;
        }

		/// <summary>
		/// Permet de récupérer la date de la création du fichier.
		/// </summary>
		/// <param name="pathFile"></param>
		/// <returns></returns>
	    private DateTime GetDateFileCreated(string pathFile)
		{
			return File.Exists(pathFile) 
				? File.GetCreationTime(pathFile) 
				: DateTime.MinValue;
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

	}
}
