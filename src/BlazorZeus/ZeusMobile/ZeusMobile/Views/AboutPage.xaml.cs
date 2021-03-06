using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	public partial class AboutPage : ContentPage
	{
		public AboutPage()
		{
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			AboutViewModel veiwModel = (AboutViewModel)BindingContext;

			await veiwModel.Auth();
			base.OnAppearing();
		}
	}
}