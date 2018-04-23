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
            IEnumerable<MovieInformation> movieInformations = new List<MovieInformation>();

            try
            {
                string urlMovies = UrlServer + API_GET_MOVIES;

                HttpClient client = new HttpClient();
                string movies = await client.GetStringAsync(urlMovies);
                movieInformations = JsonConvert.DeserializeObject<IEnumerable<MovieInformation>>(movies);
            }
            catch (Exception exception)
            {
                // TODO : Log de l'erreur pour l'acces au server.
                _logger.Error("Erreur récupération de la liste des films présent sur le serveur", exception);
            }

            return movieInformations;
        }

        /// <summary>
        /// Retourne la liste des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieInformation>> GetDessinsAnimesInformationAsync()
        {
            IEnumerable<MovieInformation> movieInformations = new List<MovieInformation>();

            try
            {
                string urlMovies = UrlServer + API_GET_DESSINS_ANIMES;

                HttpClient client = new HttpClient();
                string movies = await client.GetStringAsync(urlMovies);
                movieInformations = JsonConvert.DeserializeObject<IEnumerable<MovieInformation>>(movies);
            }
            catch (Exception exception)
            {
                // TODO : Log de l'erreur pour l'acces au server.
                _logger.Error("Erreur récupération de la liste des films présent sur le serveur", exception);
            }

            return movieInformations;
        }

        /// <summary>
        /// Télécharge le film voulu
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
        public void DownloadVideo(MovieInformation movieInformation, Func<MovieInformation, string> findGoodPlace)
        {
            try
            {
                string urlMovies = UrlServer + API_DOWNLOAD_MOVIES;
                string pathSave = Path.Combine(findGoodPlace.Invoke(movieInformation), movieInformation.FileName);
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
            }
        }

        #endregion
    }
}
