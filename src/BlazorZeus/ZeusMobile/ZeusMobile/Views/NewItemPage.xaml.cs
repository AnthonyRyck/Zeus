using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ZeusMobile.Models;
using ZeusMobile.ViewModels;

namespace ZeusMobile.Views
{
	public partial class NewItemPage : ContentPage
	{
		public Item Item { get; set; }

		public NewItemPage()
		{
			InitializeComponent();
			BindingContext = new NewItemViewModel();
		}
	}
}