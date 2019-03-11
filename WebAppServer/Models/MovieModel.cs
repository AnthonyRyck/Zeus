using System;
using MoviesLib.Entities;
using TMDbLib.Objects.Movies;

namespace WebAppServer.Models
{
    public class MovieModel
    {
        #region Properties

        /// <summary>
        /// ID du film.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Information sur le fichier
        /// </summary>
        public MovieInformation MovieInformation { get; set; }

        /// <summary>
        /// Informations venant de TmDb
        /// </summary>
        public Movie MovieTmDb { get; set; }

        /// <summary>
        /// Indicateur pour savoir si le fichier est déjà DL.
        /// </summary>
        public bool IsDownloaded { get; set; }

		/// <summary>
		/// Date d'ajout du film.
		/// </summary>
	    public DateTime DateAdded { get; set; } = DateTime.MinValue;
 
        #endregion

        public MovieModel(Guid id)
        {
            Id = id;
        }
    }
}
