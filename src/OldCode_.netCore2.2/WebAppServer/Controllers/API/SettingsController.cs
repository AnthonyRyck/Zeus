using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Settings")]
    public class SettingsController : Controller
    {
        private IMovies _moviesManager;
        //private readonly ILogger _logger;

        public SettingsController(IMovies manager)
        {
            _moviesManager = manager;
        }


        #region Public Methods

        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public MovieModel Get(Guid id)
        {
            //Log.Information("Demande liste d'affiche pour un ID.");
            return _moviesManager.GetMovie(id);

            //return await _moviesManager.GetListVideoOnTmDb(video.MovieInformation.Titre);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idVideoTmDb"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<JsonResult> ChangeVideo(Guid id, [FromBody] int idVideoTmDb)
        {
            MovieModel video = await _moviesManager.ChangeVideo(id, idVideoTmDb);
            return Json(new { poster = video.MovieTmDb.PosterPath, title = video.MovieTmDb.Title, description = video.MovieTmDb.Overview });
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="titreRecherche"></param>
		/// <returns></returns>
		[HttpPost("search")]
	    public async Task<JsonResult> GetSearchVideo([FromBody]string titreRecherche)
		{
			var videos = await _moviesManager.GetListVideoOnTmDb(titreRecherche);
			return Json(videos);
		}

        #endregion
    }
}