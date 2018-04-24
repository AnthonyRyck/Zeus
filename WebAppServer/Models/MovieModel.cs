﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
 
        #endregion

        public MovieModel(Guid id)
        {
            Id = id;
        }
    }
}
