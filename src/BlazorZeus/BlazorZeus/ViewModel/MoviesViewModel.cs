using BlazorZeus.Codes;
using BlazorZeus.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.ViewModel
{
	public class MoviesViewModel : IMoviesViewModel
	{

		#region Properties

		private IMovies MoviesManager { get; set; }

		public bool ShowConfigureMovie { get; set; }

		public IEnumerable<SearchVideoModel> SearchVideos { get; set; }

		public MovieModel MovieToChange { get; set; }
				
		public NavigationManager MyNavigationManager { get; set; }

		/// <summary>
		/// Liste de tous les films.
		/// </summary>
		public List<MovieModel> MoviesCollection { get; set; }

		#endregion

		public MoviesViewModel(NavigationManager navigation, IMovies moviesManager)
		{
			MoviesManager = moviesManager;
			MyNavigationManager = navigation;

			MoviesCollection = (MoviesManager.GetMovies().GetAwaiter().GetResult()).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public async Task ChangeMovie(MovieModel movie)
		{
			MovieToChange = movie;
			SearchVideos = await MoviesManager.GetListVideoOnTmDb(movie.MovieInformation.Titre);
			ShowConfigureMovie = true;
		}

		public async Task SelectMovie(int id)
		{
			ShowConfigureMovie = false;

			var newVideo = await MoviesManager.ChangeVideo(MovieToChange.Id, id);

			// Récupère le movie Model, et met à jour par passage de référence dans la liste.
			var tempReference = MoviesCollection.FirstOrDefault(x => x.Id == MovieToChange.Id);
			tempReference = newVideo;

			MovieToChange = null;
			SearchVideos = null;
		}

		public async Task GetNewTitle(string titre)
		{
			SearchVideos = await MoviesManager.GetListVideoOnTmDb(titre);
		}


		public void CloseConfigure()
		{
			ShowConfigureMovie = false;
		}

		public void ChangeQuality(string quality)
		{
			ShowConfigureMovie = false;

			if (!string.IsNullOrEmpty(quality))
			{
				MoviesManager.ChangeResolution(MovieToChange.Id, quality);

				// Récupère le movie Model, et met à jour par passage de référence dans la liste.
				var tempReference = MoviesCollection.FirstOrDefault(x => x.Id == MovieToChange.Id);
				tempReference.MovieInformation.Resolution = quality;
			}

			MovieToChange = null;
			SearchVideos = null;
		}

	}
}
