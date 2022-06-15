namespace ZeusMaui.Views;

public partial class MovieDetailPage : ContentPage
{
	private DetailMovieViewModel ViewModel;

	public MovieDetailPage()
	{
		InitializeComponent();
		ViewModel = ServiceHelper.GetService<DetailMovieViewModel>();
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


	//private async void OnSelectVideo(object sender, SelectedItemChangedEventArgs e)
	//{
	//	Video videoSelected = e.SelectedItem as Video;

	//	Dictionary<string, object> navigationParameter = new Dictionary<string, object>()
	//		{
	//			{ "url", "https://www.youtube.com/embed/" + videoSelected.Key }
	//		};
	//	await Shell.Current.GoToAsync(nameof(TrailerPage), navigationParameter);
	//}
}