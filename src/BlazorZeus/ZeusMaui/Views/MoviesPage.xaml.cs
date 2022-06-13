using ZeusCore;
using ZeusMaui.ViewModels;

namespace ZeusMaui.Views;

public partial class MoviesPage : ContentPage
{
	private MoviesViewModel ViewModel;

	public MoviesPage(MoviesViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
		ViewModel = viewModel;
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
		//var filmSelected = e.SelectedItem as InformationMovie;

		//var movieViewModel = new DetailMovieViewModel(filmSelected.IdMovie);

		//await Navigation.PushAsync(new MovieDetail(movieViewModel));
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

	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var filmSelected = e.CurrentSelection as InformationMovie;
		await Shell.Current.GoToAsync("//settings");

		//var movieViewModel = new DetailMovieViewModel(filmSelected.IdMovie);
		//await Navigation.PushAsync(new MovieDetail(movieViewModel));
	}
}