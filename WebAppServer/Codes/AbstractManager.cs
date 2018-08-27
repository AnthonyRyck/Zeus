using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public abstract class AbstractManager
    {
        #region Properties

        
        protected readonly TMDbClient ClientTmDb;
        protected readonly StorageManager Storage;
        protected readonly ISettings Settings;
		private readonly IMailing _mailingService;

        private Timer _timerUpdate;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        protected volatile Object Lock = new object();

        protected volatile bool IsUpdateTime = false;

        #endregion

        #region Constructeur

        protected AbstractManager(ISettings settings, IMailing mailingService)
        {
            // TODO : Mettre en paramtère pour que ce soit configurable.

			_mailingService = mailingService;
			ClientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

	        Storage = new StorageManager();
	        Settings = settings;

			// Démarre dans 5 secondes et toutes les 15 minutes.
			_timerUpdate = new Timer(TimerUpdate, null, 5000, settings.GetTimeToUpdateVideos());
        }

        #endregion

		#region Protected Methods

		protected async Task SendMailToUser(IEnumerable<MovieModel> movies)
		{
			await _mailingService.SendNewVideo(movies);
		}

		#endregion

		#region Timer Methods

		/// <summary>
		/// Méthode qui appelé lorsque le Timer arrive à la fin.
		/// </summary>
		/// <param name="state"></param>
		protected abstract void TimerUpdate(object state);

        #endregion
    }
}
