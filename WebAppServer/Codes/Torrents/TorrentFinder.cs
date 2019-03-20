using CloudFlareUtilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MoviesLib.Entities;

namespace WebAppServer.Codes.Torrents
{
    public class TorrentFinder
    {
        #region Properties

        private const string YGG_URL_SEARCH = @"https://www2.yggtorrent.gg/engine/search?name={0}&description=&file=&uploader=&category=2145&sub_category=2183&do=search";
        private const string YGG_URL_DAY_MOVIE = "https://www2.yggtorrent.gg/top/day";

        #endregion

        #region Constructeur

        public TorrentFinder()
        {
            
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Méthode pour récupérer la liste des torrents par rapport à un liste.
        /// </summary>
        /// <param name="moviesCollection"></param>
        /// <returns></returns>
        public async Task GetTorrents(IEnumerable<string> moviesCollection)
        {
            // Récupération des films sur Ygg
            IEnumerable<MovieInformation> moviesOnYgg = await GetTorrentOnYgg();

            return;
        }

        #endregion

        #region Private Methods


        private async Task<IEnumerable<MovieInformation>> GetTorrentOnYgg()
        {
            List<MovieInformation> retourMovies = new List<MovieInformation>();

            try
            {
                // Create the clearance handler.
                var handler = new ClearanceHandler
                {
                    // Optionally specify the number of retries, if clearance fails (default is 3).
                    MaxRetries = 2
                };

                // Create a HttpClient that uses the handler to bypass CloudFlare's JavaScript challange.
                var client = new HttpClient(handler);

                // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                var contentHtml = await client.GetStringAsync(YGG_URL_DAY_MOVIE);

                var doc = new HtmlDocument();
                doc.LoadHtml(contentHtml);

                List<HtmlNode> allCharactersList = doc.DocumentNode.Descendants()
                    .Where(x => x.Name == "a"
                                && x.Attributes["id"] != null
                                && x.Attributes["id"].Value.Contains("torrent_name"))
                    .ToList();

                foreach (var item in allCharactersList)
                {
                    var href = item.GetAttributeValue("href", "empty");
                    string text = item.InnerText;
                }
            }
            catch (Exception exception)
            {
                
            }

            return retourMovies;
        }

        #endregion
    }
}
