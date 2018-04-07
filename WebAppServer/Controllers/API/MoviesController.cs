using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}