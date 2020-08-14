using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorZeus.Models;

namespace BlazorZeus.Extensions
{
    public static class MovieWishModelExtension
    {
        /// <summary>
        /// Change la couleur du bouton en fonction si le film
        /// est dans la liste de souhait.
        /// </summary>
        /// <returns></returns>
        public static string ToClassButton(this MovieWishModel model)
        {
            return model.IsMovieAdded
                ? "btn-success"
                : "btn-primary";
        }

        /// <summary>
        /// Retourne le texte pour le bouton, en fonction
        /// si le film est dans la liste de souhait.
        /// </summary>
        /// <returns></returns>
        public static string ToTextContent(this MovieWishModel model)
        {
            return model.IsMovieAdded
                ? "Ajouté"
                : "Souhait";
        }


        public static string ToAjaxMethodAddOrRemoveMovie(this MovieWishModel model)
        {
            return model.IsMovieAdded
                ? $"removeMovieToWishList({model.IdVideoTmDb})"
                : $"addMovieToWishList({model.IdVideoTmDb})";
        }
    }
}
