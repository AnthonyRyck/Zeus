﻿using System;
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

        /// <summary>
        /// Retourne les informations des films, d'après le site The Movie Database.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MovieModel> GetMoviesTmDb()
        {
            List<MovieModel> listeMovieModels = null;

            // Voir en fichier de sauvegarde s'il y a des informations.
            string fullPathMovieModel = Path.Combine(_pathSauvegarde, FILE_MOVIES_MODELS);
            if (File.Exists(fullPathMovieModel))
            {
                string contentJson = File.ReadAllText(fullPathMovieModel);
                listeMovieModels = JsonConvert.DeserializeObject<List<MovieModel>>(contentJson);
            }

            return listeMovieModels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retourMovieModels"></param>
        internal void SaveMoviesModels(IEnumerable<MovieModel> retourMovieModels)
        {
            string contentJson = JsonConvert.SerializeObject(retourMovieModels);

            string pathSaveMovieModel = Path.Combine(_pathSauvegarde, FILE_MOVIES_MODELS);
            File.WriteAllText(pathSaveMovieModel, contentJson);
        }

		/// <summary>
		/// Sauvegarde la collection de série.
		/// </summary>
		/// <param name="serieCollection"></param>
	    internal void SaveSeriesModels(SerieCollection serieCollection)
	    {
			string contentJson = JsonConvert.SerializeObject(serieCollection.Get());

		    string pathSaveMovieModel = Path.Combine(_pathSauvegarde, FILE_SHOWS);
		    File.WriteAllText(pathSaveMovieModel, contentJson);
		}

	    /// <summary>
	    /// Retourne la sauvegarde des séries.
	    /// </summary>
	    /// <returns></returns>
	    internal IEnumerable<ShowModel> GetShowModel()
	    {
		    List<ShowModel> tempShows = null;

		    // Voir en fichier de sauvegarde s'il y a des informations.
		    string path = Path.Combine(_pathSauvegarde, FILE_SHOWS);
		    if (File.Exists(path))
		    {
			    string contentJson = File.ReadAllText(path);
			    tempShows = JsonConvert.DeserializeObject<List<ShowModel>>(contentJson);
		    }

		    return tempShows;
	    }

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

		#endregion

		#region Private Methods

		/// <summary>
		/// Retourne le chemin d'acces pour les fichiers de configuration.
		/// </summary>
		/// <returns></returns>
		private string GetConfigPath()
        {
            return GetFolder(@"/config");
        }

        /// <summary>
        /// Retourne le chemin d'acces pour les fichiers de sauvegarde.
        /// </summary>
        /// <returns></returns>
        private string GetSavePath()
        {
            return GetFolder(@"/save");
        }

        private string GetFolder(string folderName)
        {
            string folder = AppContext.BaseDirectory + folderName;
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
                PathMovies = new List<string> {"/app/movies"},
                PathDessinAnimes = new List<string>{"/app/animes"},
                PathShows = new List<string> {"/app/series"},
                TempsEnMillisecondPourTimerRefresh = 600000
            };
        }

        #endregion

    }
}
