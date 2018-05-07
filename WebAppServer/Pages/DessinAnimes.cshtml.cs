using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppServer.Codes;

namespace WebAppServer.Pages
{
    public class DessinAnimesModel : PageModel
    {
        #region Fields

        private IMovies _animeManager;

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> DessinAnimes { get; set; }

        #endregion

        #region Constructeur

        public DessinAnimesModel(IMovies animeManager)
        {
            _animeManager = animeManager;
        }

        #endregion


        public async void OnGet()
        {
            DessinAnimes = await _animeManager.GetDessinAnimes();
        }
    }
}