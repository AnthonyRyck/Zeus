using System;
using System.Collections.Generic;
using System.IO;
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
            IEnumerable<MovieInformation> movieInformations = new List<MovieInformation>();

            try
            {
                string urlMovies = UrlServer + API_GET_MOVIES;

                HttpClient client = new HttpClient();
                string movies = await client.GetStringAsync(urlMovies);
                movieInformations = JsonConvert.DeserializeObject<IEnumerable<MovieInformation>>(movies);
            }
            catch (Exception e)
            {
                // TODO : Log de l'erreur pour l'acces au server.
            }

            return movieInformations;
        }

        /// <summary>
        /// Télécharge le film voulu
        /// </summary>
        /// <param name="movieInformation"></param>
        /// <returns></returns>
        public void DownloadMovies(MovieInformation movieInformation)
        {
            try
            {
                string urlMovies = UrlServer + API_DOWNLOAD_MOVIES;
                string pathSave = Path.Combine(_findGoodPlace.Invoke(movieInformation), movieInformation.FileName);
                Console.WriteLine("..Chemin de destination " + pathSave);
                
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlMovies);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Console.WriteLine("..Récupération de " + movieInformation.Titre + " - " + DateTime.Now);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(movieInformation);
                    streamWriter.Write(json);
                }

                Console.WriteLine("..Json donné.");
                
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Console.WriteLine("..httpResponse - terminé." + " - " + DateTime.Now);

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
                    Console.WriteLine("..Fin de récupération.." + " - " + DateTime.Now);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception lors de la récupération du film : " + e.Message + " - " + e.StackTrace);
                // TODO : Log exception sur non récupération du film.
                // TODO : Mettre d'autres types d'exception.
            }
        }

        #endregion
    }
}
