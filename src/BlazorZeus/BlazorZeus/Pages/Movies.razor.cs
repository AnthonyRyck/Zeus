using BlazorZeus.Codes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDownloadFile;
using System.IO;

namespace BlazorZeus.Pages
{
	public partial class Movies
	{
        #region Fields

        [Inject]
        private IMovies MoviesManager { get; set; }

		[Inject]
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

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


		/// <summary>
		/// Event sur un click pour DL une vidéo.
		/// </summary>
		/// <param name="e"></param>
		/// <param idMovieSelected="id">ID du film.</param>
		public void DownloadMovie(MouseEventArgs e, Guid idMovieSelected)
		{
			var movieSelected = MoviesCollection.First(x => x.Id == idMovieSelected);
			var path = movieSelected.MovieInformation.PathFile;
			var fileName = movieSelected.MovieInformation.FileName;

			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				BlazorDownloadFileService.DownloadFile(fileName, fs, "application/octet-stream");
			}
		}

	}
}
