using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppClient.Codes;
using WebAppServer.Models;

namespace WebAppClient.Pages
{
    public class DessinAnimesModel : PageModel
    {
        #region Fields

        private IClientManager _animeManager;

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<MovieModel> DessinAnimes { get; set; }

        #endregion

        #region Constructeur

        public DessinAnimesModel(IClientManager animeManager)
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