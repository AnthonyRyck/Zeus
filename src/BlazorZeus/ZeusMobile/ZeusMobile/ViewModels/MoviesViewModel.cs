using MoviesLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZeusCore;
using ZeusMobile.Services;

namespace ZeusMobile.ViewModels
{
	public class MoviesViewModel : BaseViewModel
	{
		/// <summary>
		/// Notre liste de Fan
		/// </summary>
		public List<InformationMovie> AllMovies
		{
			get { return _allMovies; }
			set
			{
				_allMovies = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<InformationMovie> _allMovies;

		private IZeusService ZeusSvc => DependencyService.Get<IZeusService>();


		/// <summary>
		/// Charge tous les fans.
		/// </summary>
		/// <returns></returns>
		public async Task LoadMovies()
		{
			AllMovies = await ZeusSvc.GetAllMovies();
		}
	}
}
