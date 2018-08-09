using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoviesLib.Entities;
using Serilog;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Serie")]
    public class SerieController : Controller
    {
	    private IShows _showsManager;
		
		public SerieController(IShows showsManager)
		{
			_showsManager = showsManager;
		}

		[HttpGet("{idShowModel}/{saison}/{episode}")]
		public IActionResult Download(Guid idShowModel, int saison, int episode)
		{
			ShowModel show = _showsManager.GetShow(idShowModel);
			ShowInformation showInformation = show.ShowInformation.FirstOrDefault(x => x.Saison == saison
			                                                                           && x.Episode == episode);

			if (showInformation == null)
				return StatusCode(204);

			// Vérification que le fichier est toujours présent sur le disque.
			if (!System.IO.File.Exists(showInformation.PathFile))
			{
				show.RemoveVideo(showInformation);
				return StatusCode(204);
			}

			Log.Information("Récupération par Web de la série : " + showInformation.Titre);
			return DownloadVideoCore(showInformation);
		}


	    private IActionResult DownloadVideoCore(ShowInformation information)
	    {
		    IActionResult temp = null;

		    try
		    {
			    FileStream file = new FileStream(information.PathFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
				    true);

			    Func<Stream, ActionContext, Task> funcTemp = async (outputStream, context) =>
			    {
				    using (var fileStream = new WriteOnlyStreamWrapper(outputStream))
				    {
					    using (var stream = file)
					    {
						    await stream.CopyToAsync(fileStream);
					    }
				    }
			    };

			    temp = new FileCallbackResult("application/octet-stream", funcTemp)
			    {
				    FileDownloadName = information.FileName
			    };
		    }
		    catch (Exception exception)
		    {
			    Log.Error(exception, "Erreur sur la récupération de la série " + information.Titre);
			    temp = NoContent();
		    }


		    return temp;
	    }
	}
}