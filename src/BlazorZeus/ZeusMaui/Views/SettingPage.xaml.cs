using ZeusMaui.ViewModels;


namespace ZeusMaui.Views;

public partial class SettingPage : ContentPage
{
	private SettingViewModel Setting;

	public SettingPage(SettingViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
		Setting = viewModel;
	}

	private async void OnTestClicked(object sender, EventArgs e)
	{
		try
		{
			await Setting.TestServeur();
		}
		catch (Exception)
		{
			await DisplayAlert("Erreur", "Erreur sur le test du serveur", "OK");
		}
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		try
		{
			Setting.SaveServeur();
		}
		catch (Exception)
		{
			await DisplayAlert("Erreur", "Erreur sur la sauvegarde du serveur", "OK");
		}
	}

	private async void Entry_Completed(object sender, EventArgs e)
	{
		try
		{
			await Setting.TestServeur();
		}
		catch (Exception)
		{
			await DisplayAlert("Erreur", "Erreur sur le test du serveur", "OK");
		}
	}
}