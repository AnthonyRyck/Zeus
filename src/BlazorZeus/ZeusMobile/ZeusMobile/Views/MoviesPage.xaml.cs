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
	public partial class MoviesPage : ContentPage
	{
		private MoviesViewModel ViewModel;

		public MoviesPage()
		{
			InitializeComponent();
			ViewModel = BindingContext as MoviesViewModel;
		}


		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await ViewModel.LoadMovies();
		}

		private async void OnFilmSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var filmSelected = e.SelectedItem as InformationMovie;
			
			var movieViewModel = new DetailMovieViewModel(filmSelected.IdMovie);

			await Navigation.PushAsync(new MovieDetail(movieViewModel));
		}
	}
}