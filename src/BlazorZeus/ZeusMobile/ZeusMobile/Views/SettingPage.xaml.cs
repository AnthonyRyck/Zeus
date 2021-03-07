using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingPage : ContentPage
	{
		private SettingViewModel settingViewModel;

		public SettingPage()
		{
			InitializeComponent();
			settingViewModel = (SettingViewModel)BindingContext;
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
			settingViewModel.LoadSetting();
		}


		private async void OnTestClicked(object sender, EventArgs e)
		{
			await settingViewModel.TestServeur();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
			settingViewModel.SaveServeur();
		}
	}
}