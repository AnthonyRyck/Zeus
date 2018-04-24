using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MoviesLib.Entities;
using WebAppServer.Codes;
using WebAppServer.Models;

namespace WebAppServer.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesController : Controller
    {
        private IShowsAndMovies _moviesManager;
        private readonly ILogger _logger;


        #region Public Methods

        public MoviesController(IShowsAndMovies moviesManager, ILogger<MoviesController> logger)
        {
            _moviesManager = moviesManager;
            _logger = logger;
        }

        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<MovieInformation>> Get()
        {
            _logger.LogInformation("Demande de récupération des films en local.");
            return await _moviesManager.GetListMoviesLocal();
        }

        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet("dessinAnimes")]
        public async Task<IEnumerable<MovieInformation>> GetDessinAnimes()
        {
            _logger.LogInformation("Demande de récupération des dessins animés en local.");
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

            _logger.LogInformation("Récupération par API du film : " + movieInformation.Titre);

            return tempVideo;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Download(Guid id)
        {
            MovieModel movie = _moviesManager.GetMovie(id);

            _logger.LogInformation("Récupération par Web du film : " + movie.MovieInformation.Titre);
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
                _logger.LogError(exception, "Erreur sur la récupération du film " + movieInformation.Titre);
                temp = NoContent();
            }


            return temp;
        }

        #endregion

    }
}