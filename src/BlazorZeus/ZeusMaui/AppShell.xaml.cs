using ZeusMaui.Views;

namespace ZeusMaui
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(MovieDetailPage), typeof(MovieDetailPage));
			Routing.RegisterRoute(nameof(TrailerPage), typeof(TrailerPage));
		}
	}
}