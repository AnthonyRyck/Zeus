using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesLib.Entities
{
    /// <summary>
    /// Classe contenant toutes les informations sur un film.
    /// </summary>
    public class MovieInformation : IEquatable<MovieInformation>
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

        /// <summary>
        /// Chemin d'accès au fichier.
        /// </summary>
        public string PathFile { get; set; }

        #endregion

        #region Implement IEquatable

        public bool Equals(MovieInformation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(PathFile, other.PathFile);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;
            return Equals((MovieInformation)obj);
        }

        public override int GetHashCode()
        {
            return (PathFile != null ? PathFile.GetHashCode() : 0);
        }

        #endregion
    }
}
