﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WebAppServer.Models;

namespace MoviesAutomate.Codes
{
    public class StorageManager
    {
        #region Properties

        private readonly string _pathSauvegarde;

        /// <summary>
        /// Nom du fichier de sauvegarde contenant toutes les informations TmDB des films
        /// présent en local.
        /// </summary>
        private const string FILE_MOVIES_MODELS = "saveMoviesModels.json";

        private readonly string _pathConfiguration;
        private const string NAME_FILE_CONFIG = "config_app.json";

        #endregion

        #region Constructeur

        public StorageManager()
        {
            _pathConfiguration = GetConfigPath();
            _pathSauvegarde = GetSavePath();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Récupére les informations de configuration.
        /// </summary>
        /// <returns></returns>
        internal ConfigAppClient GetConfiguration()
        {
            ConfigAppClient config;

            // Tester la présence du fichier de configuration.
            string fileConfig = _pathConfiguration + @"/" + NAME_FILE_CONFIG;
            if (File.Exists(fileConfig))
            {
                string configJson = File.ReadAllText(fileConfig);
                config = JsonConvert.DeserializeObject<ConfigAppClient>(configJson);
            }
            else
            {
                config = GetDefaultConfiguration();
                string contentJson = JsonConvert.SerializeObject(config);
                File.WriteAllText(fileConfig, contentJson);
            }

            return config;
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Retourne le chemin d'acces pour les fichiers de configuration.
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath()
        {
            string localFolderConfig = AppContext.BaseDirectory + @"/config";
            if (!Directory.Exists(localFolderConfig))
            {
                Directory.CreateDirectory(localFolderConfig);
            }

            return localFolderConfig;
        }

        /// <summary>
        /// Retourne le chemin d'acces pour les fichiers de sauvegarde.
        /// </summary>
        /// <returns></returns>
        private string GetSavePath()
        {
            string folder = AppContext.BaseDirectory + @"/save";
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
        private ConfigAppClient GetDefaultConfiguration()
        {
            return new ConfigAppClient()
            {
                LanguePourTmDb = "fr-FR",
                RegionPourTmDb = "FR",
                ListeDeLangue = new List<string>()
                {
                    "FRENCH", "TRUEFRENCH", "FR"
                },
                PathMovies = new List<string> { "/moviesClient" },
                PathDessinAnimes = new List<string>() { "/dessinAnimes" },
                PathShows = "/shows",
                TempsEnMillisecondPourTimerRefresh = 60000,
                UrlServer = string.Empty
            };
        }

        #endregion
    }
}
