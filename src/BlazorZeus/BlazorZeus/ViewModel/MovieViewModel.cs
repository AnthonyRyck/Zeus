using BlazorZeus.Codes;
using BlazorZeus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.ViewModel
{
	public class MovieViewModel : IMovieViewModel
	{
		private IMovies _movieManager;

		public MovieModel MovieSelected { get; set; }

		public MovieViewModel(IMovies movies)
		{
			_movieManager = movies;
		}


		public void LoadMovieInfo(Guid idMovie)
		{
			MovieSelected = _movieManager.GetMovie(idMovie);
		}
	}
}
