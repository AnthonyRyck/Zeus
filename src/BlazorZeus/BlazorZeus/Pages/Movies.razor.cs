using BlazorZeus.Codes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Pages
{
	public partial class Movies
	{
        #region Fields

        [Inject]
        private IMovies MoviesManager { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> MoviesCollection { get; set; }

		#endregion


		protected override async Task OnInitializedAsync()
		{
            MoviesCollection = await MoviesManager.GetMovies();
		}


		//public async void OnGet()
  //      {
  //          //Log.Debug("Consultation page - Films -");
  //          MoviesCollection = await MoviesManager.GetMovies();
  //          //Log.Debug("Page Films - Movies = " + Movies.Count() + " films.");
  //      }
    }
}
