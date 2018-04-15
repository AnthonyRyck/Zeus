using System;
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

        /// <summary>
        /// Retourne la liste des dessin animes qui sont présent sur le local.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieInformation>> GetListDessinAnimesLocal();

        /// <summary>
        /// Retourne la liste des dessin animes avec toutes les informations.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieModel>> GetDessinAnimes();

        /// <summary>
        /// Indique que ce film a été téléchargé.
        /// </summary>
        /// <param name="movieInformation"></param>
        void SetMovieDownloaded(MovieInformation movieInformation);

        /// <summary>
        /// Récupère le film par rapport à son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MovieModel GetMovie(Guid id);
    }
}
