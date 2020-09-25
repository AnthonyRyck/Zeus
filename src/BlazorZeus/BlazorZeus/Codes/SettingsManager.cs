using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Codes
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

	    public uint GetTimeToUpdateVideos()
	    {
		    return _configApp.TempsEnMinutePourTimerRefresh;
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

		/// <summary>
		/// Permet de sauvegarder le fichier de settings avec des valeurs
		/// par défault.
		/// NON FINI.
		/// </summary>
		/// <param name="langueTmdb"></param>
		/// <param name="regionTmdb"></param>
		/// <param name="tempsRefresh">10 minutes de base</param>
		/// <returns></returns>
	    public async Task SaveSettings(string langueTmdb = "fr-FR", 
										string regionTmdb = "FR",
										uint tempsRefresh = 10,
										string email = "",
										string passwordMail = "")
	    {
			//TODO : Finir la méthode de sauvegarde des Settings.
		    ConfigurationApp config = new ConfigurationApp
		    {
			    LanguePourTmDb = langueTmdb,
			    RegionPourTmDb = regionTmdb,
			    TempsEnMinutePourTimerRefresh = tempsRefresh,
			    ListeDeLangue = GetLanguesVideos().ToList(),
			    PathMovies = GetPathMovies(),
			    PathDessinAnimes = GetPathDessinAnimes(),
			    PathShows = GetPathShows(),
				Mail = email,
				PasswordMail = passwordMail
			};
			
		    _configApp = await _storage.SaveConfiguration(config);
	    }

		/// <inheritdoc />
		public string GetMail()
		{
			return _configApp.Mail;
		}

		/// <inheritdoc />
		public string GetPasswordMail()
		{
			return _configApp.PasswordMail;
		}

		#endregion
	}
}
