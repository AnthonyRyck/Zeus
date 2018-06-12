﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Codes
{
    public class SettingsManager : ISettings
    {
		#region Properties

		private readonly StorageManager _storage;
	    private ConfigurationApp _configApp;

		#endregion

		#region Constructeur

		public SettingsManager()
	    {
		    _storage = new StorageManager();
		    _configApp = _storage.GetConfiguration().Result;
	    }

		#endregion

		#region Implements ISettings


		public IEnumerable<string> GetLanguesVideos()
		{
			return _configApp.ListeDeLangue;
		}

	    public int GetTimeToUpdateVideos()
	    {
		    return _configApp.TempsEnMillisecondPourTimerRefresh;
	    }

	    public IEnumerable<string> GetPathMovies()
	    {
		    return _configApp.PathMovies;
	    }

	    public IEnumerable<string> GetPathDessinAnimes()
	    {
		    return _configApp.PathDessinAnimes;
	    }

	    public IEnumerable<string> GetPathShows()
	    {
		    return _configApp.PathShows;
	    }

	    public string GetLangueTmdb()
	    {
		    return _configApp.LanguePourTmDb;
	    }

	    public string GetRegionTmdb()
	    {
		    return _configApp.RegionPourTmDb;
	    }

	    public async Task SaveSettings(string langueTmdb = "fr-FR", 
										string regionTmdb = "FR",
										int tempsRefresh = 3600000)
	    {
		    ConfigurationApp config = new ConfigurationApp
		    {
			    LanguePourTmDb = langueTmdb,
			    RegionPourTmDb = regionTmdb,
			    TempsEnMillisecondPourTimerRefresh = tempsRefresh,
				

				// TODO : Finir la méthode pour la sauvegarde.
			    ListeDeLangue = new List<string>
				{
				    "FRENCH", "TRUEFRENCH", "FR"
			    },
			    PathMovies = new List<string> { "/app/movies" },
			    PathDessinAnimes = new List<string> { "/app/animes" },
			    PathShows = new List<string> { "/app/shows" },
			};
			
		    _configApp = await _storage.SaveConfiguration(config);
	    }

	    #endregion
	}
}
