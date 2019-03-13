using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public class MovieDatabase : ITheMovieDatabase
    {
        #region Properties

        /// <summary>
        /// Client pour accéder à l'API The Movie Database.
        /// </summary>
        private readonly TMDbClient _clientTmDb;

        #endregion

        #region Constructeur

        public MovieDatabase()
        {
            _clientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };
        }

        #endregion

        #region Implement ITheMovieDatabase

        /// <inheritdoc cref="ITheMovieDatabase.GetMoviesNowPlayingAsync"/>
        public async Task<CollectionMovieWishModel> GetMoviesNowPlayingAsync(int numeroPage = 1)
        {
            var moviesTemp = await _clientTmDb.GetMovieNowPlayingListAsync("fr-FR", numeroPage);
            return BuildCollectionMovieWishModel(moviesTemp.Results, moviesTemp.Page, moviesTemp.TotalPages);
        }

        /// <inheritdoc cref="ITheMovieDatabase.GetMoviesPopularAsync"/>
        public async Task<CollectionMovieWishModel> GetMoviesPopularAsync(int numeroPage = 1)
        {
            var moviesTemp = await _clientTmDb.GetMoviePopularListAsync("fr-FR", numeroPage);
            return BuildCollectionMovieWishModel(moviesTemp.Results, moviesTemp.Page, moviesTemp.TotalPages);
        }

        /// <inheritdoc cref="ITheMovieDatabase.GetMoviesUpcomingAsync"/>
        public async Task<CollectionMovieWishModel> GetMoviesUpcomingAsync(int numeroPage = 1)
        {
            var moviesTemp = await _clientTmDb.GetMovieUpcomingListAsync("fr-FR", numeroPage);
            return BuildCollectionMovieWishModel(moviesTemp.Results, moviesTemp.Page, moviesTemp.TotalPages);
        }

        /// <inheritdoc cref="ITheMovieDatabase.GetMoviesTopRatedAsync"/>
        public async Task<CollectionMovieWishModel> GetMoviesTopRatedAsync(int numeroPage = 1)
        {
            var moviesTemp = await _clientTmDb.GetMovieTopRatedListAsync("fr-FR", numeroPage);
            return BuildCollectionMovieWishModel(moviesTemp.Results, moviesTemp.Page, moviesTemp.TotalPages);
        }

        /// <inheritdoc cref="ITheMovieDatabase.GetMovie"/>
        public async Task<MovieWishModel> GetMovie(int idMovie)
        {
            var movieTemp = await _clientTmDb.GetMovieAsync(idMovie);

            return new MovieWishModel()
            {
                UrlAffiche = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + movieTemp.PosterPath,
                IdVideoTmDb = movieTemp.Id,
                Titre = movieTemp.Title,
                OriginalTitle = movieTemp.OriginalTitle,
                Description = movieTemp.Overview,
                IsMovieAdded = false
            };
        }

        /// <inheritdoc cref="ITheMovieDatabase.GetMoviesByName"/>
        public async Task<CollectionMovieWishModel> GetMoviesByName(string name, int numeroPage = 0)
        {
            var moviesTemp = await _clientTmDb.SearchMovieAsync(name, numeroPage);
            return BuildCollectionMovieWishModel(moviesTemp.Results, moviesTemp.Page, moviesTemp.TotalPages);
        }

        #endregion

        #region Private Methods

        private CollectionMovieWishModel BuildCollectionMovieWishModel(List<SearchMovie> listMovies, int page, int totalPages)
        {
            List<MovieWishModel> retourInfo = new List<MovieWishModel>();
            foreach (var result in listMovies)
            {
                if (!string.IsNullOrEmpty(result.PosterPath))
                {
                    retourInfo.Add(new MovieWishModel()
                    {
                        UrlAffiche = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + result.PosterPath,
                        IdVideoTmDb = result.Id,
                        Titre = result.Title,
                        OriginalTitle = result.OriginalTitle,
                        Description = result.Overview,
                        IsMovieAdded = false
                    });
                }
            }

            CollectionMovieWishModel returnInformation = new CollectionMovieWishModel
            {
                Page = page,
                TotalPage = totalPages,
                MovieWishModels = retourInfo
            };

            return returnInformation;
        }
        
        #endregion

    }
}
