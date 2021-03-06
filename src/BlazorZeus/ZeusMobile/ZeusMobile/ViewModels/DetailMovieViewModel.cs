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
			set
			{
				_movie = value;
				OnNotifyPropertyChanged();
			}
		}
		private DetailMovie _movie;

		/// <summary>
		/// Pour l'affichage de la taille du film
		/// </summary>
		public string TailleFilm
		{
			get { return _tailleFilm; }
			set
			{
				_tailleFilm = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _tailleFilm;

		public List<Video> VideosPromo
		{
			get { return _videosPromo; }
			set
			{
				_videosPromo = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<Video> _videosPromo;

		/// <summary>
		/// Indique s'il y a des vidéos.
		/// </summary>
		public bool HasVideo
		{
			get { return _hasVideo; }
			set
			{
				_hasVideo = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _hasVideo;

		/// <summary>
		/// Pas de vidéo.
		/// </summary>
		public bool PasDeVideo
		{
			get { return _pasDeVideo; }
			set
			{
				_pasDeVideo = value;
				OnNotifyPropertyChanged();
			}
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

			VideosPromo = Movie.Videos;
			HasVideo = Movie.Videos.Count > 0;
			PasDeVideo = !HasVideo;
		}

	}
}
