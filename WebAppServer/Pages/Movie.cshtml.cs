using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppServer.Codes;

namespace WebAppServer.Pages
{
    public class MovieModel : PageModel
    {
        #region Fields

        private IShowsAndMovies _moviesManager;

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> Movies { get; set; }

        #endregion

        #region Constructeur

        public MovieModel(IShowsAndMovies movieManager)
        {
            _moviesManager = movieManager;
        }

        #endregion



        public async void OnGet()
        {
            Movies = await _moviesManager.GetMovies();
        }
    }
}