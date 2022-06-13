using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using ZeusCore;
using ZeusMaui.Services;

namespace ZeusMaui.ViewModels
{
	[INotifyPropertyChanged]
	public partial class MoviesViewModel
	{
		/// <summary>
		/// Notre liste de Fan
		/// </summary>
		[ObservableProperty]
		private List<InformationMovie> allMovies;

		[ObservableProperty]
		private string messageNoFilm;

		[ObservableProperty]
		private bool hasFilms;

		private IZeusService ZeusSvc;

		public MoviesViewModel(IZeusService zeusService)
		{
			ZeusSvc = zeusService;
		}


		/// <summary>
		/// Charge tous les fans.
		/// </summary>
		/// <returns></returns>
		public async Task LoadMovies()
		{
			try
			{
				var temp = await ZeusSvc.GetAllMovies();
				var tempDate = temp.OrderByDescending(movie => movie.DateAdded).ToList();

				AllMovies = tempDate;
				HasFilms = tempDate.Count > 0;
			}
			catch (Exception)
			{
				AllMovies = new List<InformationMovie>();
				throw;
			}
		}

		/// <summary>
		/// Permet de changer l'ordonnancement.
		/// </summary>
		/// <param name="ordreVoulu"></param>
		internal void ChangeOrdre(string ordreVoulu)
		{
			switch (ordreVoulu)
			{
				case "Date added":
					var tempDate = AllMovies.OrderByDescending(movie => movie.DateAdded).ToList();
					AllMovies = tempDate;
					break;

				case "Name":
					var tempName = AllMovies.OrderBy(movie => movie.Titre).ToList();
					AllMovies = tempName;
					break;

				default:
					break;
			}
		}

		public void OpenMovieDetail()
		{
			
		}
	}
}
