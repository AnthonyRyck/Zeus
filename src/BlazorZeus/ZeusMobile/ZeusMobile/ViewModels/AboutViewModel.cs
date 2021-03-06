using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ZeusMobile.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";
			OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
		}

		public ICommand OpenWebCommand { get; }



		public async Task Auth()
		{
			//var authResult = await WebAuthenticator.AuthenticateAsync(
			//							new Uri("https://192.168.1.24:45455/mobileauth"),
			//							new Uri("myapp://"));
			//
			//var accessToken = authResult?.AccessToken;
		}
	}
}