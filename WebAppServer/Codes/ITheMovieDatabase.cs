using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public interface ITheMovieDatabase
    {
        /// <summary>
        /// Retourne la liste des films qui sont sorties.
        /// </summary>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesNowPlayingAsync(int numeroPage = 1);
    }
}
