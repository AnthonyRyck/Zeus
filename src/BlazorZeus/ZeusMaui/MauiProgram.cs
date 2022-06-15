using CommunityToolkit.Maui;
using ZeusMaui.Services;
using ZeusMaui.Views;

namespace ZeusMaui
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			// Initialise the toolkit
			builder.UseMauiCommunityToolkit();

			builder.Services.AddSingleton<IZeusService, ZeusService>();

			builder.Services.AddScoped<SettingViewModel>();
			builder.Services.AddTransient<SettingPage>();

			builder.Services.AddScoped<MoviesViewModel>();
			//builder.Services.AddTransient<MoviesPage>();

			builder.Services.AddTransient<DetailMovieViewModel>();
			//builder.Services.AddTransient<MovieDetailPage>();

			return builder.Build();
		}
	}
}