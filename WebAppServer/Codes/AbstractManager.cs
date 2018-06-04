using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace WebAppServer.Codes
{
    public abstract class AbstractManager
    {
        #region Properties

        
        protected readonly TMDbClient ClientTmDb;
        protected readonly StorageManager Storage;
        protected readonly ConfigurationApp ConfigurationApp;

        private Timer _timerUpdate;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        protected volatile Object Lock = new object();

        protected volatile bool IsUpdateTime = false;

        #endregion

        #region Constructeur

        protected AbstractManager()
        {
            // TODO : Mettre en paramtère pour que ce soit configurable.
            
            ClientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

	        Storage = new StorageManager();
			ConfigurationApp = Storage.GetConfiguration();

			// Démarre dans 5 secondes et toutes les 15 minutes.
			_timerUpdate = new Timer(TimerUpdate, null, 5000, ConfigurationApp.TempsEnMillisecondPourTimerRefresh);
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
