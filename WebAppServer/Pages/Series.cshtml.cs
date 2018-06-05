using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebAppServer.Codes;

namespace WebAppServer.Pages
{
	[Authorize]
	public class SeriesModel : PageModel
    {
	    #region Fields

	    private IShows _showsManager;
	    private readonly ILogger _logger;


	    #endregion

	    #region Properties

	    /// <summary>
	    /// Liste de toutes les séries.
	    /// </summary>
	    public IEnumerable<Models.ShowModel> Series { get; set; }

	    #endregion

	    #region Constructeur

	    public SeriesModel(IShows showManager, ILogger<MovieModel> logger)
	    {
		    _showsManager = showManager;
		    _logger = logger;
	    }

	    #endregion
		public void OnGet()
        {
	        _logger.LogDebug("Consultation page - Series -");
	        Series = _showsManager.GetShows();
	        _logger.LogDebug("Page Series - Nombre de séries = " + Series.Count());
		}

	}
}