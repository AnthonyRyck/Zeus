﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoviesLib.Entities;
using Serilog;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesController : Controller
    {
        private IMovies _moviesManager;


        #region Public Methods

        public MoviesController(IMovies moviesManager)
        {
            _moviesManager = moviesManager;
        }

        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<MovieInformation>> Get()
        {
            Log.Information("Demande de récupération des films en local.");
            return await _moviesManager.GetListMoviesLocal();
        }

        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet("dessinAnimes")]
        public async Task<IEnumerable<MovieInformation>> GetDessinAnimes()
        {
            Log.Information("Demande de récupération des dessins animés en local.");
            return await _moviesManager.GetListDessinAnimesLocal();
        }
        
        /// <summary>
        /// Serves a file as ZIP.
        /// </summary>
        [HttpPost("download")]
        public IActionResult GetVideoFile([FromBody]MovieInformation movieInformation)
        {
            var tempVideo = DownloadVideoCore(movieInformation);
            _moviesManager.SetMovieDownloaded(movieInformation);

            Log.Information("Récupération par API du film : " + movieInformation.Titre);

            return tempVideo;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Download(Guid id)
        {
            MovieModel movie = _moviesManager.GetMovie(id);

            Log.Information("Récupération par Web du film : " + movie.MovieInformation.Titre);

	        if (!System.IO.File.Exists(movie.MovieInformation.PathFile))
	        {
		        _moviesManager.RemoveVideo(id);
		        return StatusCode(204);
	        }

            return DownloadVideoCore(movie.MovieInformation);
        }

        #endregion

        #region Private Methods

        private IActionResult DownloadVideoCore(MovieInformation movieInformation)
        {
            IActionResult temp = null;

            try
            {
                FileStream file = new FileStream(movieInformation.PathFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
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
                    FileDownloadName = movieInformation.FileName
                };
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Erreur sur la récupération du film " + movieInformation.Titre);
                temp = NoContent();
            }


            return temp;
        }

        #endregion

    }
}