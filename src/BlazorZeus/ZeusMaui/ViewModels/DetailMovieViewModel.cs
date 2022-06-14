using CommunityToolkit.Mvvm.ComponentModel;
using ZeusCore;
using ZeusMaui.Services;

namespace ZeusMaui.ViewModels
{
	[INotifyPropertyChanged]
	public partial class DetailMovieViewModel : IQueryAttributable
	{
		private Guid _idMovie;
		private IZeusService ZeusSvc;

		public DetailMovieViewModel(IZeusService zeusService)
		{
			ZeusSvc = zeusService;
		}

		public void SetIdMovie(Guid idMovie)
		{
			_idMovie = idMovie;
		}

		/// <summary>
		/// Film a afficher les informations.
		/// </summary>
		[ObservableProperty]
		private DetailMovie _movie;

		/// <summary>
		/// Pour l'affichage de la taille du film
		/// </summary>
		[ObservableProperty]
		private string _tailleFilm;

		[ObservableProperty]
		private List<Video> _videosPromo;

		/// <summary>
		/// Indique s'il y a des vidéos.
		/// </summary>
		[ObservableProperty]
		private bool _hasVideo;

		/// <summary>
		/// Pas de vidéo.
		/// </summary>
		[ObservableProperty]
		private bool _pasDeVideo;
				

		internal async Task LoadMovieDetail()
		{
			Movie = await ZeusSvc.GetMovie(_idMovie);
			TailleFilm = Helpers.GetSize(Movie.Size);

			VideosPromo = Movie.Videos ?? new List<Video>();
			HasVideo = VideosPromo.Count > 0;
			PasDeVideo = !HasVideo;
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
			Guid idMovie = (Guid)query["idMovie"];
			_idMovie = idMovie;
		}
	}
}
