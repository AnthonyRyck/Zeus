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
        public IEnumerable<MovieWishModel> ListMovies { get; private set; }

        /// <summary>
        /// Pour l'affichage du titre de la page.
        /// </summary>
        public string TitrePage { get; set; }

        #endregion

        #region Constructeur

        public FindWishModel(ITheMovieDatabase tmDbSite, IWish wishMaster)
        {
            _wishMaster = wishMaster;
            _database = tmDbSite;
            ListMovies = new List<MovieWishModel>();
        }

        #endregion

        #region private Methods

        private void LoadMovies(CollectionMovieWishModel moviesSite)
        {
            string userId = this.User.GetUserId();
            
            IEnumerable<WishModel> allWishes = _wishMaster.GetWishes();

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

            ListMovies = moviesSite.MovieWishModels;
        }

        #endregion

        #region Events RazorPage

        #region Get Methods

        public async Task OnGet()
        {
            await OnGetNowPlaying();
        }

        public async Task OnGetNowPlaying()
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesNowPlayingAsync();
            TitrePage = "Films actuellement au cinéma";
            LoadMovies(moviesSite);
        }

        public async Task OnGetPopular()
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesPopularAsync();
            TitrePage = "Films populaires";
            LoadMovies(moviesSite);
        }

        public async Task OnGetTopRated()
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesTopRatedAsync();
            TitrePage = "Films mieux notés";
            LoadMovies(moviesSite);
        }

        public async Task OnGetUpcoming()
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesUpcomingAsync();
            TitrePage = "Films bientôt au cinéma";
            LoadMovies(moviesSite);
        }

        #endregion

        #region Post Methods

        public async Task OnPostAddWishMovie(int idMovie)
        {
            MovieWishModel tempMovie = await _database.GetMovie(idMovie);
            tempMovie.IsMovieAdded = true;

            string userId = this.User.GetUserId();

            _wishMaster.AddMovie(tempMovie, Guid.Parse(userId));
        }

        #endregion

        #endregion

    }
}