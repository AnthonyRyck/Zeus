using BlazorZeus.Codes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDownloadFile;
using System.IO;
using BlazorZeus.Models;
using MoviesLib.Entities;
using System.Net.Http;

namespace BlazorZeus.Pages
{
	public partial class Movies
	{
        #region Fields

        [Inject]
        private IMovies MoviesManager { get; set; }

		[Inject]
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		private bool ShowConfigureMovie = false;

		private IEnumerable<SearchVideoModel> _searchVideos;
		private MovieModel _movieToChange;

		[Inject]
		public NavigationManager MyNavigationManager { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Liste de tous les films.
		/// </summary>
		public List<MovieModel> MoviesCollection { get; set; }

		#endregion


		protected override async Task OnInitializedAsync()
		{
            MoviesCollection = (await MoviesManager.GetMovies()).ToList();
		}

		/// <summary>
		/// Event sur un click pour DL une vidéo.
		/// </summary>
		/// <param name="e"></param>
		/// <param idMovieSelected="id">ID du film.</param>
		public async void DownloadMovie(MouseEventArgs e, Guid idMovieSelected)
		{
			//await Http.GetStreamAsync(MyNavigationManager.BaseUri + "/api/" + idMovieSelected.ToString());

			//var movieSelected = MoviesCollection.First(x => x.Id == idMovieSelected);
			//var path = movieSelected.MovieInformation.PathFile;
			//var fileName = movieSelected.MovieInformation.FileName;


			//using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			//{
			//	BlazorDownloadFileService.DownloadFile(fileName, fs, "application/octet-stream");
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public async Task ChangeMovie(MouseEventArgs e, MovieModel movie)
		{
			_movieToChange = movie;
			_searchVideos = await MoviesManager.GetListVideoOnTmDb(movie.MovieInformation.Titre);
			ShowConfigureMovie = true;
		}

		public async Task SelectMovie(int id)
		{
			ShowConfigureMovie = false;

			var newVideo = await MoviesManager.ChangeVideo(_movieToChange.Id, id);

			// Récupère le movie Model, et met à jour par passage de référence dans la liste.
			var tempReference = MoviesCollection.FirstOrDefault(x => x.Id == _movieToChange.Id);
			tempReference = newVideo;

			_movieToChange = null;
			_searchVideos = null;
		}

		public async Task GetNewTitle(string titre)
		{
			_searchVideos = await MoviesManager.GetListVideoOnTmDb(titre);
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
				MoviesManager.ChangeResolution(_movieToChange.Id, quality);

				// Récupère le movie Model, et met à jour par passage de référence dans la liste.
				var tempReference = MoviesCollection.FirstOrDefault(x => x.Id == _movieToChange.Id);
				tempReference.MovieInformation.Resolution = quality;
			}

			_movieToChange = null;
			_searchVideos = null;
		}


	}
}
