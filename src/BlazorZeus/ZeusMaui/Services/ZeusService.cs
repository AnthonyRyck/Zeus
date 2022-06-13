using ZeusCore;
using System.Text.Json;
using ZeusMaui.Models;

namespace ZeusMaui.Services
{
	public class ZeusService : IZeusService
	{
		private HttpClient Client;
		private Setting MySetting;

		public bool IsServerAdressOk { get; set; }

		private string PathSetting;


		public ZeusService()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			PathSetting = Path.Combine(path, "settings.info");
			LoadSetting();

			if (string.IsNullOrEmpty(MySetting.AddressServer))
			{
				IsServerAdressOk = false;
			}
			else
			{
				// Pour ignorer les erreurs SSL
				var httpClientHandler = new HttpClientHandler();
				httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

				Client = new HttpClient(httpClientHandler)
				{
					BaseAddress = new Uri(MySetting.AddressServer)
				};
				IsServerAdressOk = true;
			}
		}

		private void LoadSetting()
		{
			if (File.Exists(PathSetting))
			{
				var json = File.ReadAllText(PathSetting);
				MySetting = JsonSerializer.Deserialize<Setting>(json);
				UrlServer = MySetting.AddressServer;
			}
			else
			{
				MySetting = new Setting();
				SaveFile().GetAwaiter().GetResult();
			}

			return;
		}


		public async Task SaveServeur(string adresseSvr)
		{
			try
			{
				MySetting.AddressServer = adresseSvr;
				UrlServer = adresseSvr;
				await SaveFile();
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task SaveFile()
		{
			string content = JsonSerializer.Serialize(MySetting);
			await File.WriteAllTextAsync(PathSetting, content);
		}




		#region Implement IZeusService

		public string UrlServer { get; private set; }

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
					JsonSerializerOptions opt = new JsonSerializerOptions()
					{
						PropertyNameCaseInsensitive = true
					};

					Stream content = await response.Content.ReadAsStreamAsync();
					allMovies = await JsonSerializer.DeserializeAsync<List<InformationMovie>>(content, opt);
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
					Stream content = await response.Content.ReadAsStreamAsync();
					movie = await JsonSerializer.DeserializeAsync<DetailMovie>(content);
				}
			}
			catch (Exception)
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
		
		#endregion
	}
}
