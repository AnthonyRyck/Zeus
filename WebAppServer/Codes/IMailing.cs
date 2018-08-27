using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
	public interface IMailing
	{
		/// <summary>
		/// Envoie un mail aux membres sur les nouveautés.
		/// </summary>
		/// <param name="newVideos"></param>
		/// <returns></returns>
		Task SendNewVideo(IEnumerable<MovieModel> newVideos);
	}
}
