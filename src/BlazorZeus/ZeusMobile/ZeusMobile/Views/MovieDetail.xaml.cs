using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeusCore;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieDetail : ContentPage
	{
		private readonly DetailMovieViewModel _viewModel;

		public MovieDetail(DetailMovieViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = viewModel;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			try
			{
				await _viewModel.LoadMovieDetail();
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur la récupération des informations du film", "OK");
			}
		}

		private async void OnSelectVideo(object sender, SelectedItemChangedEventArgs e)
		{
			Video videoSelected = e.SelectedItem as Video;
			await Navigation.PushAsync(new VideoPage("https://www.youtube.com/embed/" + videoSelected.Key));
		}
	}
}