using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.TvShows;
using BlazorZeus.Models;

namespace BlazorZeus.Codes
{
    public abstract class AbstractManager
    {
        #region Properties

        
        protected readonly TMDbClient ClientTmDb;
        protected readonly StorageManager Storage;
        protected readonly ISettings Settings;

        /// <summary>
        /// Object utilisé pour faire les locks.
        /// </summary>
        protected volatile Object Lock = new object();

        #endregion

        #region Constructeur

        protected AbstractManager(ISettings settings)
        {
            // TODO : Mettre en paramtère pour que ce soit configurable.

			ClientTmDb = new TMDbClient("034c4e19f68e958da378fd83c9e6f450")
            {
                DefaultLanguage = "fr-FR",
                DefaultCountry = "FR"
            };

	        Storage = new StorageManager();
	        Settings = settings;
        }

        #endregion
		
    }
}
