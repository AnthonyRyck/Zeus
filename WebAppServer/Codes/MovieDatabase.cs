using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
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

        
        //public async Task<IEnumerable<MovieWishModel>> GetMoviesNowPlayingAsync(int numeroPage = 1)
        //{
        //    var moviesTemp = await _clientTmDb.GetMovieNowPlayingListAsync("fr-FR", numeroPage);

        //    List<MovieWishModel> retourInfo = new List<MovieWishModel>();
        //    foreach (var result in moviesTemp.Results)
        //    {
        //        if (!string.IsNullOrEmpty(result.PosterPath))
        //        {
        //            retourInfo.Add(new MovieWishModel()
        //            {
        //                UrlAffiche = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + result.PosterPath,
        //                IdVideoTmDb = result.Id,
        //                Titre = result.Title,
        //                OriginalTitle = result.OriginalTitle,
        //                Description = result.Overview,
        //                IsMovieAdded = false
        //            });
        //        }
        //    }

        //    return retourInfo;
        //}

        /// <summary>
        /// Permet de récupérer la liste des films qui sont en cours au cinéma.
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionMovieWishModel> GetMoviesNowPlayingAsync(int numeroPage = 1)
        {
            var moviesTemp = await _clientTmDb.GetMovieNowPlayingListAsync("fr-FR", numeroPage);

            List<MovieWishModel> retourInfo = new List<MovieWishModel>();
            foreach (var result in moviesTemp.Results)
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
                Page = moviesTemp.Page,
                TotalPage = moviesTemp.TotalPages,
                MovieWishModels = retourInfo
            };

            return returnInformation;
        }

        #endregion

    }
}
