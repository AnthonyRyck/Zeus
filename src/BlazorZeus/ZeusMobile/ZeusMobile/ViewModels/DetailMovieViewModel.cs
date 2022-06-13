using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZeusCore;
using ZeusMobile.Services;

namespace ZeusMobile.ViewModels
{
	public class DetailMovieViewModel : BaseViewModel
	{
		private Guid _idMovie;

		private IZeusService ZeusService => DependencyService.Get<IZeusService>();

		/// <summary>
		/// Film a afficher les informations.
		/// </summary>
		public DetailMovie Movie
		{
			get { return _movie; }
			set { SetProperty(ref _movie, value); }
		}
		private DetailMovie _movie;

		/// <summary>
		/// Pour l'affichage de la taille du film
		/// </summary>
		public string TailleFilm
		{
			get { return _tailleFilm; }
			set { SetProperty(ref _tailleFilm, value); }
		}
		private string _tailleFilm;

		public List<Video> VideosPromo
		{
			get { return _videosPromo; }
			set { SetProperty(ref _videosPromo, value); }
		}
		private List<Video> _videosPromo;

		/// <summary>
		/// Indique s'il y a des vidéos.
		/// </summary>
		public bool HasVideo
		{
			get { return _hasVideo; }
			set { SetProperty(ref _hasVideo, value); }
		}
		private bool _hasVideo;

		/// <summary>
		/// Pas de vidéo.
		/// </summary>
		public bool PasDeVideo
		{
			get { return _pasDeVideo; }
			set { SetProperty(ref _pasDeVideo, value); }
		}
		private bool _pasDeVideo;



		public DetailMovieViewModel(Guid idMovie)
		{
			_idMovie = idMovie;
		}

		internal async Task LoadMovieDetail()
		{
			Movie = await ZeusService.GetMovie(_idMovie);
			TailleFilm = Helpers.GetSize(Movie.Size);

			VideosPromo = Movie.Videos ?? new List<Video>();
			HasVideo = VideosPromo.Count > 0;
			PasDeVideo = !HasVideo;
		}

	}
}
