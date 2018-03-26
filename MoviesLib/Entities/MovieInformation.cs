using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesLib.Entities
{
    /// <summary>
    /// Classe contenant toutes les informations sur un film.
    /// </summary>
    public class MovieInformation
    {
        #region Properties

        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Titre { get; set; }

        /// <summary>
        /// Année de sortie du film
        /// </summary>
        public string Annee { get; set; }

        /// <summary>
        /// Résolution du film (ex: 1080p)
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// Qualité du film (ex: HDTV, LD,...)
        /// </summary>
        public string Qualite { get; set; }

        /// <summary>
        /// Langue du film.
        /// </summary>
        public string Langage { get; set; }
        

        #endregion
    }
}
