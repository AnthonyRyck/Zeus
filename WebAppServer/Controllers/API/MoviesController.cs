﻿using System;
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
using Microsoft.Net.Http.Headers;
using MoviesLib.Entities;
using WebAppServer.Codes;

namespace WebAppServer.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesController : Controller
    {
        private IShowsAndMovies _moviesManager;

        public MoviesController(IShowsAndMovies moviesManager)
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
            return await _moviesManager.GetListMoviesLocal();
        }

        /// <summary>
        /// Serves a file as ZIP.
        /// </summary>
        [HttpPost("download")]
        public IActionResult GetMovieFile([FromBody]MovieInformation movieInformation)
        {
            FileStream file = new FileStream(movieInformation.PathFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                true);

            Func<Stream, ActionContext, Task> funcTemp = async (outputStream, context) =>
            {
                using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
                {
                    var zipEntry = zipArchive.CreateEntry(movieInformation.Titre);
                    using (var zipStream = zipEntry.Open())
                    {
                        using (var stream = file)
                        {
                            await stream.CopyToAsync(zipStream);
                        }
                    }
                }
            };

            var temp = new FileCallbackResult("application/octet-stream", funcTemp)
            {
                FileDownloadName = movieInformation.FileName
            };

            return temp;
        }

    }
}