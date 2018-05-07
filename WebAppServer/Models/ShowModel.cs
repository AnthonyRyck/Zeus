using System.Collections.Generic;
using MoviesLib.Entities;
using TMDbLib.Objects.TvShows;

namespace WebAppServer.Models
{
    public class ShowModel
    {
        #region Properties

        /// <summary>
        /// C'est la série. Toutes les informations sur la série : saisons, épisodes,...
        /// </summary>
        public TvShow TvShow { get; set; }

        /// <summary>
        /// Liste des saisons par rapport aux séries présents sur le disque.
        /// </summary>
        public List<TvSeason> TvSeasons { get; set; }

        /// <summary>
        /// Liste des épisodes par rapport au vidéo sur le disque.
        /// </summary>
        public List<TvEpisode> TvEpisodes { get; set; }

        /// <summary>
        /// Contient les informations du fichier de la série.
        /// </summary>
        public List<ShowInformation> ShowInformation { get; set; }

        #endregion

        #region Public Methods

        

        #endregion
    }
}
