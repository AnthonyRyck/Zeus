﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoviesLib.Entities;
using Newtonsoft.Json;
using TMDbLib.Objects.Movies;
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
        /// Nom du fichier de sauvegarde pour les informations de films en locale.
        /// </summary>
        private const string FILE_MOVIES_INFORMATIONS = "saveMoviesInformations.json";

        /// <summary>
        /// Nom du fichier de sauvegarde pour les informations de séries en locale.
        /// </summary>
        private const string FILE_SHOWS_INFORMATIONS = "saveShowsInformations.json";

        /// <summary>
        /// Nom du fichier de sauvegarde contenant toutes les informations TmDB des films
        /// présent en local.
        /// </summary>
        private const string FILE_MOVIES_MODELS = "saveMoviesModels.json";

        private string _pathConfiguration;
        private const string NAME_FILE_CONFIG = "config_app.json";

        /// <summary>
        /// Liste de tous les films présents sur le "serveur".
        /// </summary>
        public List<MovieInformation> MovieInformationsCollection { get; private set; }

        /// <summary>
        /// Func pour la récupération des films sur le local.
        /// </summary>
        private Func<string, IEnumerable<MovieInformation>> _funcGetMovies;

        #endregion

        #region Constructeur

        public StorageManager(Func<string, IEnumerable<MovieInformation>> getMovies)
        {
            _pathConfiguration = GetConfigPath();
            _pathSauvegarde = GetSavePath();
            _funcGetMovies = getMovies;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Récupère la liste de film qui est sauvegarder en local.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MovieInformation> GetMoviesOnLocal(string pathFolderMovies)
        {
            // Si c'est null, c'est que l'application n'a pas encore fait de 
            return MovieInformationsCollection ?? (MovieInformationsCollection = LoadMovies(pathFolderMovies));
        }

        /// <summary>
        /// Récupére les informations de configuration.
        /// </summary>
        /// <returns></returns>
        internal ConfigurationApp GetConfiguration()
        {
            ConfigurationApp config;

            // Tester la présence du fichier de configuration.
            string fileConfig = _pathConfiguration + @"/" + NAME_FILE_CONFIG;
            if (File.Exists(fileConfig))
            {
                string configJson = File.ReadAllText(fileConfig);
                config = JsonConvert.DeserializeObject<ConfigurationApp>(configJson);
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
                DirectoryInfo folderConfig = Directory.CreateDirectory(localFolderConfig);

                // TODO : Copier le fichier de configuration par défault.
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
        /// Charge le fichiers de sauvegarde, ou a défault va rechercher les films
        /// sur le local.
        /// </summary>
        /// <returns></returns>
        private List<MovieInformation> LoadMovies(string pathFolderMovies)
        {
            List<MovieInformation> tempMovies;

            // Vérification du fichier de sauvegarde.
            string fileMovies = _pathSauvegarde + @"/" + FILE_MOVIES_INFORMATIONS;
            if (File.Exists(fileMovies))
            {
                try
                {
                    tempMovies = JsonConvert.DeserializeObject<List<MovieInformation>>(fileMovies);
                }
                catch (Exception e)
                {
                    // TODO : Mettre en log.

                    tempMovies = GetMoviesInformationOnLocal(pathFolderMovies).ToList();
                    SaveMovies(tempMovies, fileMovies);
                }
            }
            else
            {
                // Si aucun fichier, récupération des fichiers sur le local.
                tempMovies = GetMoviesInformationOnLocal(pathFolderMovies).ToList();
                SaveMovies(tempMovies, fileMovies);
            }

            return tempMovies;
        }

        /// <summary>
        /// Récupère les informations des films sur le local.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MovieInformation> GetMoviesInformationOnLocal(string pathFolderMovies)
        {
            var resultMovies = _funcGetMovies.Invoke(pathFolderMovies);
            return resultMovies;
        }

        /// <summary>
        /// Permet de sauvegarde en json la liste des films présent sur le local.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="pathFile"></param>
        private void SaveMovies(IEnumerable<MovieInformation> movies, string pathFile)
        {
            try
            {
                string contentJson = JsonConvert.SerializeObject(movies);
                File.WriteAllText(pathFile, contentJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                PathMovies = "/movies",
                PathShows = "/shows"
            };
        }

        #endregion


        
    }
}
