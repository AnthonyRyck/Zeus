using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using WebAppServer.Codes;

namespace WebAppServer.Pages.Videos
{
	[Authorize]
    public class MovieModel : PageModel
    {
        #region Fields

        private readonly IMovies _moviesManager;

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> Movies { get; set; }

        #endregion

        #region Constructeur

        public MovieModel(IMovies movieManager)
        {
            _moviesManager = movieManager;
        }

        #endregion


        public async void OnGet()
        {
			Log.Debug("Consultation page - Films -");
            Movies = await _moviesManager.GetMovies();
			Log.Debug("Page Films - Movies = " + Movies.Count() + " films.");
        }
    }
}