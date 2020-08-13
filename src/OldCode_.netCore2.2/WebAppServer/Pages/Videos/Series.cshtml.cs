using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using WebAppServer.Codes;

namespace WebAppServer.Pages.Videos
{
	[Authorize]
	public class SeriesModel : PageModel
    {
	    #region Fields

	    private IShows _showsManager;

	    #endregion

	    #region Properties

	    /// <summary>
	    /// Liste de toutes les séries.
	    /// </summary>
	    public IEnumerable<Models.ShowModel> Series { get; set; }

	    #endregion

	    #region Constructeur

	    public SeriesModel(IShows showManager)
	    {
		    _showsManager = showManager;
	    }

	    #endregion

		public void OnGet()
        {
	        Log.Debug("Consultation page - Series -");
	        Series = _showsManager.GetShows();
	        Log.Debug("Page Series - Nombre de séries = " + Series.Count());
		}

	}
}