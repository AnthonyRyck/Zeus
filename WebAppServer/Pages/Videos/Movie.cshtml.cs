using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MoviesLib.Entities;
using WebAppServer.Codes;

namespace WebAppServer.Pages.Videos
{
	[Authorize]
    public class MovieModel : PageModel
    {
        #region Fields

        private IMovies _moviesManager;
        private readonly ILogger _logger;


        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> Movies { get; set; }

        #endregion

        #region Constructeur

        public MovieModel(IMovies movieManager, ILogger<MovieModel> logger)
        {
            _moviesManager = movieManager;
            _logger = logger;
        }

        #endregion



        public async void OnGet()
        {
            _logger.LogDebug("Consultation page - Films -");
            Movies = await _moviesManager.GetMovies();
            _logger.LogDebug("Page Films - Movies = " + Movies.Count() + " films.");
        }
    }
}