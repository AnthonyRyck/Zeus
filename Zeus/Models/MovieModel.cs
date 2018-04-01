using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib.Entities;
using TMDbLib.Objects.Movies;

namespace Zeus.Models
{
    public class MovieModel
    {
        #region Properties

        /// <summary>
        /// Information sur le fichier
        /// </summary>
        public MovieInformation MovieInformation { get; set; }
        public Movie MovieTmDb { get; set; }

        #endregion
    }
}
