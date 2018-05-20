﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib;
using MoviesLib.Entities;
using TMDbLib.Objects.TvShows;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public class ShowsManager : AbstractManager, IShows
    {
        #region Properties

        private ShowManager _seriesManager;
        private SerieCollection _serieCollection;

        #endregion

        #region Constructeur

        public ShowsManager()
        {
            _seriesManager = new ShowManager();

			var tempCollectionShowModels = Storage.GetShowModel();
	        if (tempCollectionShowModels != null)
	        {
				_serieCollection = new SerieCollection();
		        _serieCollection.Set(tempCollectionShowModels);
	        }
		}

        #endregion

	    #region Private Methods

		/// <summary>
		/// Permet de créer un nouveau ShowModel.
		/// </summary>
		/// <param name="serie"></param>
		/// <returns></returns>
	    private async Task<ShowModel> CreateNewShowModel(ShowInformation serie)
	    {
			ShowModel showModel = new ShowModel();
			
		    var temp = await ClientTmDb.SearchTvShowAsync(serie.Titre);
		    await Task.Delay(500);

			if (temp.Results.Count > 0)
			{
				var tempSerieTrouve = temp.Results[0];

				// Récupération des informations de TmDb.
				TvShow tvShow = await ClientTmDb.GetTvShowAsync(tempSerieTrouve.Id);
				await Task.Delay(500);
				TvSeason saison = await ClientTmDb.GetTvSeasonAsync(tempSerieTrouve.Id, serie.Saison);
				await Task.Delay(500);
				TvEpisode episode = await ClientTmDb.GetTvEpisodeAsync(tempSerieTrouve.Id, serie.Saison, serie.Episode);

				showModel.TvShow = tvShow;
				showModel.TvSeasons.Add(saison);
				showModel.TvEpisodes.Add(episode);
				showModel.ShowInformation.Add(serie);
			}

			return showModel;
	    }

	    #endregion

        #region Timer Method

        protected override async void TimerUpdate(object state)
        {
            if (IsUpdateTime)
                return;
			
            IsUpdateTime = true;

            // Récupération des séries en locale.
            List<ShowInformation> seriesOnLocal = new List<ShowInformation>();
            foreach (var path in ConfigurationApp.PathShows)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                IEnumerable<ShowInformation> tempSeries = _seriesManager.GetShowsInformation(path);

                if (tempSeries.Any())
                    seriesOnLocal.AddRange(tempSeries);
            }

	        // Suppression des items en mémoire qui ne sont plus en local.
	        _serieCollection?.UpdateShowInformations(seriesOnLocal);

	        if (_serieCollection == null)
	            _serieCollection = new SerieCollection();

            foreach (ShowInformation serieLocal in seriesOnLocal)
            {
	            await Task.Delay(1000);

	            Guid idShow = _serieCollection.GetIdShow(serieLocal.Titre);

	            if (idShow == Guid.Empty)
	            {
					ShowModel showModel = await CreateNewShowModel(serieLocal);
		            _serieCollection.Add(showModel);
				}
	            else
	            {
					// Si la saison est connu.
		            if (_serieCollection.HaveSeason(idShow, serieLocal.Saison))
		            {
						// Si Episode est non connu.
			            if (!_serieCollection.HaveEpisode(idShow, serieLocal.Saison, serieLocal.Episode))
			            {
				            int idSerie = _serieCollection.GetIdSerieTmDb(idShow);
				            TvEpisode episode = await ClientTmDb.GetTvEpisodeAsync(idSerie, serieLocal.Saison, serieLocal.Episode);
				            
							_serieCollection.AddEpisode(idShow, episode, serieLocal);
			            }
					}
		            else
		            {
			            // Cas ou il ne connait pas la saison.
			            int idSerie = _serieCollection.GetIdSerieTmDb(idShow);
			            TvSeason saison = await ClientTmDb.GetTvSeasonAsync(idSerie, serieLocal.Saison);
			            await Task.Delay(500);
			            TvEpisode episode = await ClientTmDb.GetTvEpisodeAsync(idSerie, serieLocal.Saison, serieLocal.Episode);

						_serieCollection.AddSaison(idShow, saison, episode, serieLocal);
		            }
	            }
            }
            
            // Sauvegarde
            Storage.SaveSeriesModels(_serieCollection);
            IsUpdateTime = false;
        }

		#endregion

		#region Implement IShows

		/// <summary>
		/// Récupère la liste des séries.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ShowModel> GetShows()
		{
			return _serieCollection.Get();
		}

		/// <summary>
		/// Récupère le ShowModel par rapport à l'ID donnée.
		/// </summary>
		/// <param name="idshowmodel"></param>
		/// <returns></returns>
	    public ShowModel GetShow(Guid idshowmodel)
		{
			return _serieCollection.GetShowModel(idshowmodel);
		}

	    #endregion

	}
}