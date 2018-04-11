using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MoviesLib.Entities;
using Newtonsoft.Json;

namespace MoviesAutomate.Codes
{
    public class MoviesServer
    {
        #region Properties

        public string UrlServer { get; }

        private const string API_GET_MOVIES = "api/movies";
        private const string API_DOWNLOAD_MOVIES = "api/movies/download";

        /// <summary>
        /// C'est ce Func qui donne l'endroit ou sauvegarder le film.
        /// </summary>
        private Func<MovieInformation, string> _findGoodPlace;

        #endregion

        #region Constructeur

        public MoviesServer(string urlServer, Func<MovieInformation, string> findGoodPlace)
        {
            UrlServer = urlServer;
            _findGoodPlace = findGoodPlace;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retourne la liste des films présent sur le serveur.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MovieInformation>> GetMoviesInformationAsync()
        {
            string urlMovies = UrlServer + API_GET_MOVIES;

            HttpClient client = new HttpClient();
            string movies = await client.GetStringAsync(urlMovies);
            IEnumerable<MovieInformation> movieInformations = JsonConvert.DeserializeObject<IEnumerable<MovieInformation>>(movies);

            return movieInformations;
        }

        /// <summary>
        /// Télécharge le film voulu
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
        public async Task DownloadMovies(MovieInformation movieInformation)
        {
            string urlMovies = UrlServer + API_DOWNLOAD_MOVIES;
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, urlMovies);

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(movieInformation), Encoding.UTF8, "application/json");

            // Envoie la requete au serveur
            HttpResponseMessage response = await client.SendAsync(requestMessage);

            // Reception du message.
            Stream responseString = await response.Content.ReadAsStreamAsync();
            responseString.Position = 0;

            using (var fileStream = File.Create(_findGoodPlace.Invoke(movieInformation) + @"\" + movieInformation.FileName))
            {
                await responseString.CopyToAsync(fileStream);
            }
        }

        #endregion
    }
}
