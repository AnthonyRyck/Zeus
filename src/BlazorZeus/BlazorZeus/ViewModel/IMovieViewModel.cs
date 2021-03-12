using BlazorZeus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.ViewModel
{
	public interface IMovieViewModel
	{
		MovieModel MovieSelected { get; set; }

		void LoadMovieInfo(Guid idMovie);
	}
}
