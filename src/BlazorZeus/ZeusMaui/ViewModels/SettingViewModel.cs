using CommunityToolkit.Mvvm.ComponentModel;
using ZeusMaui.Services;

namespace ZeusMaui.ViewModels
{
	[INotifyPropertyChanged]
	public partial class SettingViewModel
	{
		/// <summary>
		/// Affiche le résultat du test.
		/// </summary>
		[ObservableProperty]
		private string resultTest;

		/// <summary>
		/// Indique si le test est Ok.
		/// </summary>
		[ObservableProperty]
		private bool isTestOk;

		/// <summary>
		/// Indique que la sauvegarde est OK
		/// </summary>
		[ObservableProperty]
		private bool isSaveOk;

		/// <summary>
		/// Adresse du serveur à tester.
		/// </summary>
		[ObservableProperty]
		private string adresseServer;


		private IZeusService ZeusService;

		public SettingViewModel(IZeusService service)
		{
			ZeusService = service;
			AdresseServer = ZeusService.UrlServer;
		}

		public async Task TestServeur()
		{
			if (!HaveSlash(AdresseServer))
				AdresseServer += "/";

			IsTestOk = await ZeusService.TestServerUrl(AdresseServer);

			if (IsTestOk)
			{
				ResultTest = "Connexion serveur OK.";
			}
			else
			{
				ResultTest = "Pas de connexion au serveur demandé réussie.";
			}
		}

		public async void SaveServeur()
		{
			await ZeusService.SaveServeur(AdresseServer);
			IsSaveOk = true;
		}


		private bool HaveSlash(string adresseServer)
		{
			return adresseServer[adresseServer.Length - 1] == '/';
		}
	}
}
