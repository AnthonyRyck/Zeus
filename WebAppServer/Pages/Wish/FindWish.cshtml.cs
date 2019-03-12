using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppServer.Codes;
using WebAppServer.Codes.Wish;
using WebAppServer.Extensions;
using WebAppServer.Models;

namespace WebAppServer.Pages.Wish
{
    [Authorize]
    public class FindWishModel : PageModel
    {
        #region Properties

        private readonly IWish _wishMaster;
        private readonly ITheMovieDatabase _database;
        
        /// <summary>
        /// Liste des films qui sont en diffusion au cinéma.
        /// </summary>
        public IEnumerable<MovieWishModel> MoviesOnPlaying { get; private set; }

        #endregion

        #region Constructeur

        public FindWishModel(ITheMovieDatabase tmDbSite, IWish wishMaster)
        {
            _wishMaster = wishMaster;
            _database = tmDbSite;
            MoviesOnPlaying = new List<MovieWishModel>();
        }

        #endregion

        #region Public Methods



        #endregion

        #region Events RazorPage

        public async Task OnGet()
        {
            string userId = this.User.GetUserId();

            CollectionMovieWishModel moviesSite = await _database.GetMoviesNowPlayingAsync();
            IEnumerable<WishModel> allWishes = await _wishMaster.GetWishes();

            if (allWishes != null && allWishes.Any())
            {
                // Récupération des souhaits de l'utilisateur
                var listeMoviesCurrentUser = allWishes.Where(x => x.HasUserId(userId)).ToList();

                foreach (var movies in moviesSite.MovieWishModels)
                {
                    if (listeMoviesCurrentUser.Any(x => x.Movie.IdVideoTmDb == movies.IdVideoTmDb))
                        movies.IsMovieAdded = true;
                }
            }

            MoviesOnPlaying = moviesSite.MovieWishModels;
        }

        public async Task OnPostAddWishMovie(int idMovie)
        {
            MovieWishModel tempMovie = await _database.GetMovie(idMovie);
            tempMovie.IsMovieAdded = true;

            string userId = this.User.GetUserId();

            _wishMaster.AddMovie(tempMovie, Guid.Parse(userId));
        }

        public void OnPost(int idMovie)
        {
            var stop = true;
        }

        #endregion

    }
}