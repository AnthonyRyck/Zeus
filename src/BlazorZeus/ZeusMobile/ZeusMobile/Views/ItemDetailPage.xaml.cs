using System.ComponentModel;
using Xamarin.Forms;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		public ItemDetailPage()
		{
			InitializeComponent();
			BindingContext = new ItemDetailViewModel();
		}
	}
}