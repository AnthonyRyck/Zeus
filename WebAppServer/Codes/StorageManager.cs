using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MoviesLib.Entities;
using Newtonsoft.Json;
using Serilog;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    /// <summary>
    /// Classe de gestion pour les sauvegardes, lectures,... des
    /// fichiers de configuration, des films,...
    /// </summary>
    public class StorageManager
    {
        #region Properties

        private string _pathSauvegarde;

        /// <summary>
        /// Nom du fichier de sauvegarde pour les informations de séries en locale.
        /// </summary>
        private const string FILE_SHOWS = "saveShowsInformations.json";

        /// <summary>
        /// Nom du fichier de sauvegarde contenant toutes les informations TmDB des films
        /// présent en local.
        /// </summary>
        private const string FILE_MOVIES_MODELS = "saveMoviesModels.json";

        /// <summary>
        /// Nom du fichier de sauvegarde contenant toutes
        /// les souhaits de films.
        /// présent en local.
        /// </summary>
        private const string FILE_WISH_MODELS = "saveWishModels.json";

        private string _pathConfiguration;
        private const string NAME_FILE_CONFIG = "config_app.json";

        /// <summary>
        /// Func pour la récupération des films sur le local.
        /// </summary>
        private Func<string, TypeVideo, IEnumerable<MovieInformation>> _funcGetMovies;

        #endregion

        #region Constructeur

	    public StorageManager()
	    {
			_pathConfiguration = GetConfigPath();
		    _pathSauvegarde = GetSavePath();
		}

		#endregion

		#region Public Methods

		public void SetFunc(Func<string, TypeVideo, IEnumerable<MovieInformation>> getMovies)
		{
			_funcGetMovies = getMovies;
		}

		#endregion

		#region Internal Methods
        

        #region WishList

        /// <summary>
        /// Retourne la liste de souhait de film.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<WishModel> GetWishList()
        {
            IEnumerable<WishModel> temp = Get<WishModel>(FILE_WISH_MODELS);
            return temp ?? new List<WishModel>();
        }

        /// <summary>
        /// Permet de faire la sauvegarde de liste de souhait.
        /// </summary>
        /// <param name="listSouhait"></param>
        internal void SaveWishModels(IEnumerable<WishModel> listSouhait)
        {
            SaveContent(listSouhait, FILE_WISH_MODELS);
        }

        #endregion

        #region Movies

        /// <summary>
        /// Retourne les informations des films, d'après le site The Movie Database.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MovieModel> GetMoviesTmDb()
        {
            return Get<MovieModel>(FILE_MOVIES_MODELS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retourMovieModels"></param>
        internal void SaveMoviesModels(IEnumerable<MovieModel> retourMovieModels)
        {
            SaveContent(retourMovieModels, FILE_MOVIES_MODELS);
        }

        #endregion

        #region Series

        /// <summary>
        /// Sauvegarde la collection de série.
        /// </summary>
        /// <param name="serieCollection"></param>
        internal void SaveSeriesModels(SerieCollection serieCollection)
        {
            SaveContent(serieCollection.Get(), FILE_SHOWS);
        }

        /// <summary>
        /// Retourne la sauvegarde des séries.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ShowModel> GetShowModel()
        {
            return Get<ShowModel>(FILE_SHOWS);
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Permet de sauvegarder la configuration donnée.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal async Task<ConfigurationApp> SaveConfiguration(ConfigurationApp config)
        {
            string fileConfig = Path.Combine(_pathConfiguration, NAME_FILE_CONFIG);
            string contentJson = JsonConvert.SerializeObject(config);
            await File.WriteAllTextAsync(fileConfig, contentJson);

            return config;
        }

        /// <summary>
        /// Récupére les informations de configuration.
        /// </summary>
        /// <returns></returns>
        internal async Task<ConfigurationApp> GetConfiguration()
        {
            ConfigurationApp config;

            // Tester la présence du fichier de configuration.
            string fileConfig = Path.Combine(_pathConfiguration, NAME_FILE_CONFIG);
            if (File.Exists(fileConfig))
            {
                try
                {
                    string configJson = await File.ReadAllTextAsync(fileConfig);
                    config = JsonConvert.DeserializeObject<ConfigurationApp>(configJson);
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Exception levé dans la méthode GetConfiguration.");

                    config = GetDefaultConfiguration();
                    await SaveConfiguration(config);
                }
            }
            else
            {
                config = GetDefaultConfiguration();
                await SaveConfiguration(config);
            }

            return config;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Retourne le chemin d'acces pour les fichiers de configuration.
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath()
        {
            return GetFolder(@"config");
        }

        /// <summary>
        /// Retourne le chemin d'acces pour les fichiers de sauvegarde.
        /// </summary>
        /// <returns></returns>
        private string GetSavePath()
        {
            return GetFolder(@"save");
        }

        private string GetFolder(string folderName)
        {
            string folder = Path.Combine(AppContext.BaseDirectory,folderName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        /// <summary>
        /// Permet de créer une configuration par défault.
        /// </summary>
        /// <returns></returns>
        private ConfigurationApp GetDefaultConfiguration()
        {
            return new ConfigurationApp()
            {
                LanguePourTmDb = "fr-FR",
                RegionPourTmDb = "FR",
                ListeDeLangue = new List<string>()
                {
                    "FRENCH", "TRUEFRENCH", "FR"
                },
                //PathMovies = new List<string> { "/app/movies" },
                PathMovies = new List<string> { Path.Combine(@"C:\Docker\Zeus\movies") },
                PathDessinAnimes = new List<string>{"/app/animes"},
                PathShows = new List<string> {"/app/series"},
                TempsEnMillisecondPourTimerRefresh = 600000,
				Mail = string.Empty,
				PasswordMail = string.Empty
            };
        }

        /// <summary>
        /// Permet de sauvegardé le contenu sous format JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentToSave"></param>
        /// <param name="fileName"></param>
        private void SaveContent<T>(T contentToSave, string fileName)
        {
            string contentJson = JsonConvert.SerializeObject(contentToSave);

            string pathSave = Path.Combine(_pathSauvegarde, fileName);
            File.WriteAllText(pathSave, contentJson);
        }

        /// <summary>
        /// Retourne la liste des vidéos du fichier demandé.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nomFichier"></param>
        /// <returns></returns>
        private IEnumerable<T> Get<T>(string nomFichier)
        {
            List<T> tempVideos = null;

            // Voir en fichier de sauvegarde s'il y a des informations.
            string path = Path.Combine(_pathSauvegarde, nomFichier);
            if (File.Exists(path))
            {
                string contentJson = File.ReadAllText(path);
                tempVideos = JsonConvert.DeserializeObject<List<T>>(contentJson);
            }

            return tempVideos;
        }

        #endregion

    }
}
