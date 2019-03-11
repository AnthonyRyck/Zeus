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

        public async Task OnGet()
        {
            string userId = this.User.GetUserId();

            CollectionMovieWishModel moviesSite = await _database.GetMoviesNowPlayingAsync();
            IEnumerable<WishModel> allWishes = await _wishMaster.GetWishes();

            if (allWishes != null && allWishes.Any())
            {
                // Récupération des souhaits de l'utilisateur
                var listeMoviesCurrentUser = allWishes.Where(x => x.HasUserId(userId)).ToList();

                // Jointure entre la liste de l'utilisateur et celle du site pour mettre la valeur à true
                // sur les films ajoutés en souhait.
                var temp = listeMoviesCurrentUser.Join(moviesSite.MovieWishModels, userWish => userWish.MovieTmDb.Id, 
                    site => site.IdVideoTmDb,
                    (userWish, site) => site);

                foreach (MovieWishModel customModel in temp)
                {
                    customModel.IsMovieAdded = true;
                }

                MoviesOnPlaying = temp.ToList();
            }
            else
            {
                MoviesOnPlaying = moviesSite.MovieWishModels;
            }
        }

    }
}