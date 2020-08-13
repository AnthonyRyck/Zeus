using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Pages.Videos
{
	[Authorize]
	public class DetailSerieModel : PageModel
    {
	    private IShows _showManager;
	    public ShowModel Serie { get; private set; }

        public void OnGet()
        {
	        var stop = true;
        }

		#region Constructeur

		public DetailSerieModel(IShows showManager)
		{
			_showManager = showManager;
		}

		#endregion
		

		public void OnGetSaison(Guid idshowmodel)
	    {
		    Serie = _showManager.GetShow(idshowmodel);
	    }
    }
}