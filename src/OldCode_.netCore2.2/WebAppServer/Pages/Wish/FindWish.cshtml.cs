using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Donne le nom du type Handler que la page doit utiliser
        /// pour faire les boutons Next/Previous
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// Numéro de la prochaine page
        /// </summary>
        public int NextPage { get; set; }

        /// <summary>
        /// Numéro de la page précédente
        /// </summary>
        public int PreviousPage { get; set; }

        /// <summary>
        /// Indiqua s'il y a une prochaine page 
        /// </summary>
        public string HaveNextPage { get; set; }

        /// <summary>
        /// Indique s'il y a une page précédente.
        /// </summary>
        public string HavePreviousPage { get; set; }

        /// <summary>
        /// Nom du film recherché.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string NameMovie { get; set; }

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

            UpdatePageProperties(moviesSite);
        }


        private void UpdatePageProperties(CollectionMovieWishModel moviesSite)
        {
            // Traitement pour le bouton Previous Page
            if (moviesSite.Page - 1 >= 1)
            {
                HavePreviousPage = string.Empty;
                PreviousPage = moviesSite.Page - 1;
            }
            else
            {
                HavePreviousPage = "disabled";
                PreviousPage = 1;
            }

            // Traitement pour le bouton Next Page
            if (moviesSite.Page + 1 <= moviesSite.TotalPage)
            {
                HaveNextPage = string.Empty;
                NextPage = moviesSite.Page + 1;
            }
            else
            {
                HaveNextPage = "disabled";
                NextPage = moviesSite.TotalPage;
            }
        }

        #endregion

        #region Events RazorPage

        #region Get Methods

        public async Task OnGet()
        {
            await OnGetNowPlaying();
        }

        public async Task OnGetNowPlaying(int attendeeId = 1)
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesNowPlayingAsync(attendeeId);
            TitrePage = "Films actuellement en salle";
            Handler = "NowPlaying";
            LoadMovies(moviesSite);
        }

        public async Task OnGetPopular(int attendeeId = 1)
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesPopularAsync(attendeeId);
            TitrePage = "Films populaires";
            Handler = "Popular";
            LoadMovies(moviesSite);
        }

        public async Task OnGetTopRated(int attendeeId = 1)
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesTopRatedAsync(attendeeId);
            TitrePage = "Films mieux notés";
            Handler = "TopRated";
            LoadMovies(moviesSite);
        }

        public async Task OnGetUpcoming(int attendeeId = 1)
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesUpcomingAsync(attendeeId);
            TitrePage = "Films prochainement";
            Handler = "Upcoming";
            LoadMovies(moviesSite);
        }
        
        public async Task OnGetMoviesByName()
        {
            CollectionMovieWishModel moviesSite = await _database.GetMoviesByName(NameMovie);
            TitrePage = $"Recherche pour \"{NameMovie}\"";
            Handler = "MoviesByName";
            LoadMovies(moviesSite);

            HavePreviousPage = "disabled";
            HaveNextPage = "disabled";
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

        public async Task OnPostRemoveWishMovie(int idMovie)
        {
            MovieWishModel tempMovie = await _database.GetMovie(idMovie);
            tempMovie.IsMovieAdded = false;

            string userId = this.User.GetUserId();
            _wishMaster.RemoveMovie(tempMovie, Guid.Parse(userId));
        }

        #endregion

        #endregion

    }
}