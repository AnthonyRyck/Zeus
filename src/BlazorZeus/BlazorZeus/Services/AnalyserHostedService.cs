﻿using BlazorZeus.Codes;
using BlazorZeus.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesLib;
using MoviesLib.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorZeus.Services
{
	public class AnalyserHostedService : IHostedService, IDisposable
	{

		#region Properties

		private Timer _timer;

		//private MovieManager _movieManager;

		private readonly IServiceScopeFactory _scopeFactory;
		public ISettings Settings { get; set; }

		#endregion

		#region Constructeur

		public AnalyserHostedService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		#endregion

		#region Implement Interfaces

		/// <summary>
		/// Lancement du service
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task StartAsync(CancellationToken cancellationToken)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				Settings = scope.ServiceProvider.GetRequiredService<ISettings>();

				//_movieManager = new MovieManager(Settings.GetLanguesVideos().ToArray());
				//_timer = new Timer(Analyse, null, 0, Settings.GetTimeToUpdateVideos());

				var tempsEnMillisecond = Convert.ToUInt32(TimeSpan.FromMinutes(Settings.GetTimeToUpdateVideos()).TotalMilliseconds);
				_timer = new Timer(Analyse, null, 0, tempsEnMillisecond);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Arrêt du service
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		#endregion

		#region Private methods

		private void Analyse(object state)
		{
			// Récupération des films en locale.
			using (var scope = _scopeFactory.CreateScope())
			{
				IMovies moviesManager = scope.ServiceProvider.GetRequiredService<IMovies>();

				moviesManager.AnalysePaths().Wait();
			}
		}

		#endregion



	}
}
