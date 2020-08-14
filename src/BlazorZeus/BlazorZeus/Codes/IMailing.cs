using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.TvShows;
using BlazorZeus.Models;

namespace BlazorZeus.Codes
{
	public interface IMailing
	{
		/// <summary>
		/// Envoie un mail aux membres sur les nouveautés.
		/// </summary>
		/// <param name="newVideos"></param>
		/// <returns></returns>
		Task SendNewVideo(IEnumerable<MovieModel> newVideos);

		/// <summary>
		/// Envoie un mail aux membres sur les nouvelles séries.
		/// </summary>
		/// <param name="nouveauteSeries"></param>
		/// <returns></returns>
		Task SendNewVideo(IEnumerable<KeyValuePair<TvSeason, TvEpisode>> nouveauteSeries);
	}
}
