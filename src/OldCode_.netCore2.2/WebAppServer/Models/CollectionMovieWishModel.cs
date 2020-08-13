using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class CollectionMovieWishModel
    {
        /// <summary>
        /// Page en cours
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Nombre total de page
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Liste des films dans la page en cours.
        /// </summary>
        public List<MovieWishModel> MovieWishModels { get; set; }

    }
}
