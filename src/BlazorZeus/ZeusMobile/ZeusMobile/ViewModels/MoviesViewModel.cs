﻿using MoviesLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZeusCore;
using ZeusMobile.Services;

namespace ZeusMobile.ViewModels
{
	public class MoviesViewModel : BaseViewModel
	{
		private IZeusService ZeusSvc => DependencyService.Get<IZeusService>();
		
		/// <summary>
		/// Notre liste de Fan
		/// </summary>
		public List<InformationMovie> AllMovies
		{
			get { return _allMovies; }
			set { SetProperty(ref _allMovies, value); }
		}
		private List<InformationMovie> _allMovies;

		public string MessageNoFilm
		{
			get { return _messageNoFilm; }
			set { SetProperty(ref _messageNoFilm, value); }
		}
		private string _messageNoFilm;

		public bool HasFilms
		{
			get { return _hasFilms; }
			set { SetProperty(ref _hasFilms, value); }
		}
		private bool _hasFilms;


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
				case "Par date d'ajout":
					var tempDate = AllMovies.OrderByDescending(movie => movie.DateAdded).ToList();
					AllMovies = tempDate;
					break;

				case "Par nom":
					var tempName = AllMovies.OrderBy(movie => movie.Titre).ToList();
					AllMovies = tempName;
					break;

				default:
					break;
			}
		}
	}
}
