using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        }

        #endregion

        #region Timer Method

        protected override void TimerUpdate(object state)
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









            // Sauvegarde

            IsUpdateTime = false;
        }

        #endregion

    }
}
