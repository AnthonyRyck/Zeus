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
			try
			{
				await ViewModel.LoadMovies();
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur la récupération de la liste des films", "OK");
			}
		}

		private async void OnFilmSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var filmSelected = e.SelectedItem as InformationMovie;
			
			var movieViewModel = new DetailMovieViewModel(filmSelected.IdMovie);

			await Navigation.PushAsync(new MovieDetail(movieViewModel));
		}

		/// <summary>
		/// Sur le changement de sélection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPickerSelectedChanged(object sender, EventArgs e)
		{
			var picker = (Picker)sender;
			int selectedIndex = picker.SelectedIndex;

			if (selectedIndex != -1)
			{
				ViewModel.ChangeOrdre(picker.SelectedItem.ToString());
			}
		}

	}
}