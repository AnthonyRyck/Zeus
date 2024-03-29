using ZeusCore;
using ZeusMaui.ViewModels;

namespace ZeusMaui.Views;

public partial class MoviesPage : ContentPage
{
	private MoviesViewModel ViewModel;

	public MoviesPage()
	{
		InitializeComponent();
		ViewModel = ServiceHelper.GetService<MoviesViewModel>();
		BindingContext = ViewModel;
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
		if(e.CurrentSelection.Count == 1)
		{
			var filmSelected = e.CurrentSelection[0] as InformationMovie;
			await ViewModel.OpenMovieDetail(filmSelected);
		}
	}
}