using MoviesLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZeusCore;

namespace ZeusMobile.Services
{
	public class ZeusService : IZeusService
	{
		private HttpClient Client;

		public bool IsServerAdressOk { get; set; }

		public ZeusService()
		{
			string address = App.SettingManager.Setting.AddressServer;

			if (string.IsNullOrEmpty(address))
			{
				IsServerAdressOk = false;
				address = "http://www.google.com/";
				return;
			}

			// Pour ignorer les erreurs SSL
			var httpClientHandler = new HttpClientHandler();
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

			Client = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(address)
			};
		}

		public async Task<List<InformationMovie>> GetAllMovies()
		{
			List<InformationMovie> allMovies = new List<InformationMovie>();

			if (!IsServerAdressOk)
				return allMovies;

			try
			{
				HttpResponseMessage response = await Client.GetAsync("api/Movies/allmovies");
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					allMovies = JsonConvert.DeserializeObject<List<InformationMovie>>(content);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return allMovies;
		}


		public async Task<DetailMovie> GetMovie(Guid idMovie)
		{
			DetailMovie movie = new DetailMovie();

			if (!IsServerAdressOk)
				return movie;

			try
			{
				HttpResponseMessage response = await Client.GetAsync("api/Movies/info/" + idMovie.ToString());

				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					movie = JsonConvert.DeserializeObject<DetailMovie>(content);
				}
			}
			catch (Exception ex)
			{
				//Debug.WriteLine(@"\tERROR {0}", ex.Message);
			}

			return movie;
		}



		public async Task<bool> TestServerUrl(string addresseServer)
		{
			bool testResult = false;

			try
			{
				var httpClientHandler = new HttpClientHandler();
				httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

				HttpClient clientTest = new HttpClient(httpClientHandler)
				{
					BaseAddress = new Uri(addresseServer)
				};

				HttpResponseMessage response = await clientTest.GetAsync("api/Movies/testconnect/");

				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					
					if (content == "Connexion OK")
						testResult = true;
				}
			}
			catch (Exception ex)
			{
				//Debug.WriteLine(@"\tERROR {0}", ex.Message);
			}

			return testResult;
		}

		public void ChangeServerAddress(string addressServer)
		{
			// Pour ignorer les erreurs SSL
			var httpClientHandler = new HttpClientHandler();
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };


			Client = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(addressServer)
			};

			IsServerAdressOk = true;
		}
	}
}
