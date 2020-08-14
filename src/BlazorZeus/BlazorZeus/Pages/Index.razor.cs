using BlazorZeus.Codes;
using BlazorZeus.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Pages
{
	public partial class Index : ComponentBase
	{
		#region Properties

		[Inject]
		private IMovies MovieManager { get; set; }

		public IEnumerable<MovieModel> MoviesOrdered { get; set; } = new List<MovieModel>();
		public IEnumerable<MovieModel> AnimesOrdered { get; set; } = new List<MovieModel>();


		#endregion

		#region Constructeur

		public Index()
		{
			
		}

		#endregion

		protected async override Task OnInitializedAsync()
		{
			// Récupération de la liste de films.
			var allMovies = await MovieManager.GetMovies();
			if (allMovies.Any())
			{
				var tempMovies = allMovies.OrderByDescending(x => x.DateAdded).ToList();

				MoviesOrdered = tempMovies.Count >= 6
					? tempMovies.Take(6)
					: tempMovies.Take(tempMovies.Count);
			}

			// Récupération des dessins animés.
			var allAnimes = await MovieManager.GetDessinAnimes();
			if (allAnimes.Any())
			{
				var temp = allAnimes.OrderByDescending(x => x.DateAdded).ToList();

				AnimesOrdered = temp.Count >= 6
					? temp.Take(6)
					: temp.Take(temp.Count);
			}
		}



	}
}
