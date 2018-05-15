using System;
using System.Collections.Generic;
using System.Linq;
using MoviesLib.Entities;
using TMDbLib.Objects.TvShows;

namespace WebAppServer.Models
{
    public class ShowModel
    {
        #region Properties

	    public Guid IdShowModel { get; set; }

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

	    #region Constructeur

	    public ShowModel()
	    {
		    IdShowModel = Guid.NewGuid();
		    TvSeasons = new List<TvSeason>();
			TvEpisodes = new List<TvEpisode>();
		    ShowInformation = new List<ShowInformation>();
		}

		#endregion

		#region Public Methods



		#endregion

		#region Internal Methods

		/// <summary>
		/// Retourne une indication si le ShowModel connait la saison demandé.
		/// </summary>
		/// <param name="numberSeason"></param>
		/// <returns></returns>
		internal bool HaveSeason(short numberSeason)
		{
			return TvSeasons.Any(x => x.SeasonNumber == numberSeason);
		}

		/// <summary>
		/// Retourne une indication si le ShowModel connait l'épisode de la saison demandé.
		/// </summary>
		/// <param name="seasonNumber"></param>
		/// <param name="episodeNumber"></param>
		/// <returns></returns>
		internal bool HaveEpisode(short seasonNumber, short episodeNumber)
	    {
			return TvEpisodes.Any(x => x.SeasonNumber == seasonNumber
									&& x.EpisodeNumber == episodeNumber);
		}

		/// <summary>
		/// Permet d'enlever une vidéo du model, ainsi que les informations 
		/// d'épisode TmDb (voir Saison si plus d'épisode).
		/// </summary>
		/// <param name="video"></param>
	    internal void RemoveVideo(ShowInformation video)
	    {
		    ShowInformation.Remove(video);

		    TvEpisode episodeToDelete = TvEpisodes.FirstOrDefault(x => x.SeasonNumber == video.Saison
		                                                               && x.EpisodeNumber == video.Episode);
		    if (episodeToDelete != null)
		    {
			    TvEpisodes.Remove(episodeToDelete);

			    if (TvEpisodes.All(x => x.SeasonNumber != video.Saison))
			    {
				    TvSeason saison = TvSeasons.FirstOrDefault(x => x.SeasonNumber == video.Saison);

				    if (saison != null)
				    {
					    TvSeasons.Remove(saison);
				    }
			    }
		    }
	    }

		#endregion


	}
}
