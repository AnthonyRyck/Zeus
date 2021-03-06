using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieDetail : ContentPage
	{
		private readonly DetailMovieViewModel _viewModel;

		public MovieDetail(DetailMovieViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = viewModel;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			await _viewModel.LoadMovieDetail();
		}
	}
}