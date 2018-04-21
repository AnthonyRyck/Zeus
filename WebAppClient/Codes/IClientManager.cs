using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppServer.Models;

namespace WebAppClient.Codes
{
    public interface IClientManager
    {
        /// <summary>
        /// Récupère la liste des dessins animés.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieModel>> GetDessinAnimes();

        /// <summary>
        /// Retourne la liste des films avec toutes les informations de chaque film.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieModel>> GetMovies();
    }
}
