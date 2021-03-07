using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeusMobile.Services;
using ZeusMobile.Views;

namespace ZeusMobile
{
	public partial class App : Application
	{
		
		public static SettingManager SettingManager { get; set; }

		public App()
		{
			InitializeComponent();

			SettingManager = new SettingManager();
			SettingManager.LoadSetting();

			DependencyService.Register<IZeusService, ZeusService>();
			MainPage = new AppShell();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
