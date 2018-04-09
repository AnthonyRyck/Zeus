﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib.Entities;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public interface IShowsAndMovies
    {
        /// <summary>
        /// Retourne la liste des films qui sont présent sur le local.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieInformation>> GetListMoviesLocal();

        /// <summary>
        /// Retourne la liste des films avec toutes les informations de chaque film.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieModel>> GetMovies();
    }
}