using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using log4net;
using MoviesLib.Entities;
using Newtonsoft.Json;

namespace MoviesAutomate.Codes
{
    public class MoviesServer
    {
        #region Properties

        public string UrlServer { get; }

        private const string API_GET_MOVIES = "api/movies";
        private const string API_GET_DESSINS_ANIMES = "api/movies/dessinAnimes";
        private const string API_DOWNLOAD_MOVIES = "api/movies/download";
        private static readonly ILog _logger
            = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        #region Constructeur

        public MoviesServer(string urlServer)
        {
            UrlServer = urlServer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retourne la liste des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieInformation>> GetMoviesInformationAsync()
        {
            string url = UrlServer + API_GET_MOVIES;
            return await GetVideosInformationAsync(url);
        }

        /// <summary>
        /// Retourne la liste des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieInformation>> GetDessinsAnimesInformationAsync()
        {
            string url = UrlServer + API_GET_DESSINS_ANIMES;
            return await GetVideosInformationAsync(url);
        }

        /// <summary>
        /// Télécharge le film voulu
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <param name="findGoodPlace">Func qui va donner le meilleur endroit pour faire la sauvegarde.</param>
        /// <returns></returns>
        public void DownloadVideo(MovieInformation movieInformation, Func<MovieInformation, string> findGoodPlace)
        {
            string pathSave = Path.Combine(findGoodPlace.Invoke(movieInformation), movieInformation.FileName);

            try
            {
                string urlMovies = UrlServer + API_DOWNLOAD_MOVIES;
                _logger.Debug("..Chemin de destination " + pathSave);
                
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlMovies);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                _logger.Debug("..Récupération de " + movieInformation.Titre + " - " + DateTime.Now);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(movieInformation);
                    streamWriter.Write(json);
                }
                _logger.Debug("..Json donné.");
                
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                _logger.Debug("..httpResponse - terminé.");

                _logger.Info("Début récupération - " + movieInformation.Titre);
                using (var stream = httpResponse.GetResponseStream())
                {
                    using (FileStream fileStream = new FileStream(pathSave, FileMode.Create))
                    {
                        byte[] buffer = new byte[65536];
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {

                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                _logger.Info("Fin récupération - " + movieInformation.Titre);
            }
            catch (Exception exception)
            {
                _logger.Error("Exception lors de la récupération du film", exception);
                if (File.Exists(pathSave))
                {
                    File.Delete(pathSave);
                    _logger.Warn("Suppression du fichier - " + pathSave);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retourne la liste des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<MovieInformation>> GetVideosInformationAsync(string urlApi)
        {
            IEnumerable<MovieInformation> movieInformations = new List<MovieInformation>();

            try
            {
                HttpClient client = new HttpClient();
                string movies = await client.GetStringAsync(urlApi);
                movieInformations = JsonConvert.DeserializeObject<IEnumerable<MovieInformation>>(movies);
            }
            catch (Exception exception)
            {
                _logger.Error("Erreur récupération de la liste des vidéos présent sur le serveur - " + urlApi, exception);
            }

            return movieInformations;
        }

        #endregion
    }
}
