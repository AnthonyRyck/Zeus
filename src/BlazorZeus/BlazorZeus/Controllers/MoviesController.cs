using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorZeus.Codes;
using BlazorZeus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesLib.Entities;
using Serilog;
using ZeusCore;

namespace BlazorZeus.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
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

        [HttpGet("allmovies")]
        public async Task<IEnumerable<InformationMovie>> GetInformationVideo()
        {
            List<InformationMovie> retourMovies = new List<InformationMovie>();

            var temp = await _moviesManager.GetMovies();

            foreach (MovieModel item in temp)
            {
                InformationMovie movie = new InformationMovie()
                {
                    IdMovie = item.Id,
                    Titre = item.MovieInformation.Titre,
                    DateAdded = item.DateAdded,
                    PosterPath = item.MovieTmDb.PosterPath,
                };

                retourMovies.Add(movie);
            }

            return retourMovies;
        }

        [HttpGet("info")]
        public async Task<DetailMovie> GetVideoInformation(Guid idMovie)
        {
            MovieModel filmSelected = _moviesManager.GetMovie(idMovie);

            DetailMovie detailMovie = new DetailMovie()
            {
                IdMovie = filmSelected.Id,
                Titre = filmSelected.MovieInformation.Titre,
                Annee = filmSelected.MovieInformation.Annee,
                Resolution = filmSelected.MovieInformation.Resolution,
                Qualite = filmSelected.MovieInformation.Qualite,
                FileName = filmSelected.MovieInformation.FileName,
                Size = filmSelected.MovieInformation.Size,
                DateAdded = filmSelected.DateAdded,
                PosterPath = filmSelected.MovieTmDb.PosterPath,
                Overview = filmSelected.MovieTmDb.Overview
            };

            return detailMovie;
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
        public IActionResult GetVideoFile([FromBody] MovieInformation movieInformation)
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
                Log.Logger.Information("Téléchargement de " + movieInformation.FileName);

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
