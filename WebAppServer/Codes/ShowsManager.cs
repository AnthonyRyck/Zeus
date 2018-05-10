using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoviesLib;
using MoviesLib.Entities;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public class ShowsManager : AbstractManager
    {
        #region Properties

        private ShowManager _seriesManager;
        private List<ShowModel> _showModelCollection;

        #endregion

        #region Constructeur

        public ShowsManager()
        {
            _seriesManager = new ShowManager();

            // TODO : Mettre le chargement de _showModelCollection.
            //var tempCollectionShowModels = Storage.GetShowsTmDb();
            //if (tempCollectionShowModels != null)
            //    _showModelCollection = tempCollectionShowModels.ToList();
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

            List<ShowModel> listeToDelete = new List<ShowModel>();

            if (_showModelCollection != null)
            {
                // Détermination des différences entre ce qui est présent sur le disque
                // et ce qui est connu en mémoire.
                foreach (ShowModel local in _showModelCollection)
                {
                    //if (!videosOnLocal.Contains(local.MovieInformation))
                    //{
                        listeToDelete.Add(local);
                    //}
                }

                lock (Lock)
                {
                    // Suppression des séries qui n'existant plus
                    foreach (var toDelete in listeToDelete)
                    {
                        _showModelCollection.Remove(toDelete);
                    }
                }
            }

            if (_showModelCollection == null)
                _showModelCollection = new List<ShowModel>();

            // Voir s'il y a des rajouts.
            List<ShowInformation> tempInformations = _showModelCollection.SelectMany(x => x.ShowInformation).ToList();
            List<ShowInformation> listeToAdd = new List<ShowInformation>();

            foreach (ShowInformation local in seriesOnLocal)
            {
                if (!tempInformations.Contains(local))
                {
                    listeToAdd.Add(local);
                }
            }

            //var tempAddMovieModels = await GetSerieDbInformation(listeToAdd);
            //lock (Lock)
            //{
            //    _showModelCollection.AddRange(tempAddMovieModels);
            //}
            
            // Sauvegarde
            //TODO : Faire la sauvegarde.
            //Storage.SaveSeriesModels(_showModelCollection);
            IsUpdateTime = false;
        }

        #endregion

    }
}
