using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public interface IShows
    {
		/// <summary>
		/// Récupère la liste des séries.
		/// </summary>
		/// <returns></returns>
	    IEnumerable<ShowModel> GetShows();

		/// <summary>
		/// Récupère le ShowModel par rapport à son ID.
		/// </summary>
		/// <param name="idshowmodel"></param>
		/// <returns></returns>
	    ShowModel GetShow(Guid idshowmodel);
    }
}
