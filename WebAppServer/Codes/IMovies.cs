using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesLib.Entities;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public interface IMovies
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
		/// Récupère une liste de vidéo provenant de TmDb pour un titre donnée.
		/// </summary>
		/// <param name="titre"></param>
		/// <returns></returns>
		Task<IEnumerable<SearchVideoModel>> GetListVideoOnTmDb(string titre);

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

        /// <summary>
        /// Permet de faire le changement d'information pour la video donnée
        /// en ID, par le contenu donnée par l'id de TmDb.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idVideoTmDb"></param>
        /// <returns></returns>
        Task<MovieModel> ChangeVideo(Guid id, int idVideoTmDb);
		
		/// <summary>
		/// Permet d'enlever une vidéo dans la collection.
		/// </summary>
		/// <param name="id"></param>
	    void RemoveVideo(Guid id);
	}
}
