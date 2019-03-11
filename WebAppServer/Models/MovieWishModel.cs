using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class MovieWishModel
    {
        /// <summary>
        /// URL pour l'image de l'affiche.
        /// </summary>
        public string UrlAffiche { get; set; }

        /// <summary>
        /// ID du film par rapport au site TheMovieDataBase
        /// </summary>
        public int IdVideoTmDb { get; set; }

        /// <summary>
        /// Titre du film en francais.
        /// </summary>
        public string Titre { get; set; }

        /// <summary>
        /// Titre original du film.
        /// </summary>
        public string OriginalTitle { get; set; }

        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicateur si le film est ajouté dans la liste de souhait.
        /// </summary>
        public bool IsMovieAdded { get; set; }



        #region Public Methods

        public string ToClassButton()
        {
            return IsMovieAdded
                ? "btn-success"
                : "btn-primary";
        }

        public string ToTextContent()
        {
            return IsMovieAdded
                ? "Ajouté"
                : "Ajouter";
        }

        #endregion
    }
}
