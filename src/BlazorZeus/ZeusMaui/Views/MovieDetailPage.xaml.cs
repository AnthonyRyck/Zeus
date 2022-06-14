using ZeusMaui.ViewModels;

namespace ZeusMaui.Views;

public partial class MovieDetailPage : ContentPage
{
	private DetailMovieViewModel ViewModel;

	public MovieDetailPage(DetailMovieViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		this.BindingContext = ViewModel;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();

		try
		{
			await ViewModel.LoadMovieDetail();
		}
		catch (Exception)
		{
			await DisplayAlert("Erreur", "Erreur sur la récupération des informations du film", "OK");
		}
	}


	private async void OnSelectVideo(object sender, SelectedItemChangedEventArgs e)
	{
		//Video videoSelected = e.SelectedItem as Video;
		//await Navigation.PushAsync(new VideoPage("https://www.youtube.com/embed/" + videoSelected.Key));
	}
}