using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Pages
{
	[Authorize]
	public class IndexModel : PageModel
    {
	    #region Properties

	    //private readonly ILogger _logger;
	    private readonly IMovies _movieManager;
	    //private readonly IShows _showManager;

	    public IEnumerable<MovieModel> MoviesOrdered { get; set; } = new List<MovieModel>();
	    public IEnumerable<MovieModel> AnimesOrdered { get; set; } = new List<MovieModel>();

		#endregion

		#region Constructeur

		public IndexModel(IMovies movies)
	    {
		    //_logger = logger;
		    _movieManager = movies;
			//_showManager = shows;
		}

	    #endregion

        public async void OnGet()
        {
			// Récupération de la liste de films.
	        var allMovies = await _movieManager.GetMovies();
	        if (allMovies.Any())
	        {
		        var tempMovies = allMovies.OrderByDescending(x => x.DateAdded).ToList();

		        MoviesOrdered = tempMovies.Count >= 6
			        ? tempMovies.Take(6)
					: tempMovies.Take(tempMovies.Count);
	        }

			// Récupération des dessins animés.
			var allAnimes = await _movieManager.GetDessinAnimes();
	        if (allAnimes.Any())
	        {
		        var temp = allAnimes.OrderByDescending(x => x.DateAdded).ToList();

		        AnimesOrdered = temp.Count >= 6
			        ? temp.Take(6)
			        : temp.Take(temp.Count);
	        }

			// Récupération des séries.


		}
    }
}
