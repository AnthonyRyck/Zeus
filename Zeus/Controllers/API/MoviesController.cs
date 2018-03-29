using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesLib.Entities;

namespace Zeus.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesController : Controller
    {
        /// <summary>
        /// Permet la récupération des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<MovieInformation> Get()
        {
            return new List<MovieInformation>();
        }
    }
}