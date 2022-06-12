using ZeusMaui.Services;
using ZeusMaui.ViewModels;
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

			builder.Services.AddScoped<IZeusService, ZeusService>();

			builder.Services.AddScoped<SettingViewModel>();
			builder.Services.AddTransient<SettingPage>();



			return builder.Build();
		}
	}
}