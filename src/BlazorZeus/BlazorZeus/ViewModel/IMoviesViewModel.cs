using BlazorZeus.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorZeus.ViewModel
{
	public interface IMoviesViewModel
	{
		List<MovieModel> MoviesCollection { get; set; }
		NavigationManager MyNavigationManager { get; set; }
		bool ShowConfigureMovie { get; set; }
		IEnumerable<SearchVideoModel> SearchVideos { get; set; }
		MovieModel MovieToChange { get; set; }

		Task ChangeMovie(MovieModel movie);
		void ChangeQuality(string quality);
		void CloseConfigure();
		Task GetNewTitle(string titre);
		Task SelectMovie(int id);

		Task OpenMovie(Guid idMovie);
	}
}