using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMDbLib.Objects.TvShows;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Pages
{
    public class DetailEpisodeSerieModel : PageModel
    {
	    private IShows _showManager;
		public ShowModel Serie { get; private set; }

	    public  IEnumerable<TvEpisode> Episodes { get; private set; }

	    public TvSeason Season { get; private set; }

		#region Constructeur

		public DetailEpisodeSerieModel(IShows showManager)
	    {
		    _showManager = showManager;
	    }

	    #endregion


	    public void OnGetEpisode(Guid idserie, int saison)
	    {
		    Serie = _showManager.GetShow(idserie);
		    Season = Serie.GetSeason(saison);
			Episodes = Serie.GetEpisodes(saison);
	    }

    }
}