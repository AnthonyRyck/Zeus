using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppClient.Codes;

namespace WebAppClient.Pages
{
    public class MoviesModel : PageModel
    {
        private IClientManager _clientManager;

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<WebAppServer.Models.MovieModel> Movies { get; set; }

        #endregion

        public MoviesModel(IClientManager manager)
        {
            _clientManager = manager;
        }


        public void OnGet()
        {

        }
    }
}