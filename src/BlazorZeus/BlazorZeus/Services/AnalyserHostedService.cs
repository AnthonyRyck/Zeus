using BlazorZeus.Codes;
using BlazorZeus.Data;
using BlazorZeus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorZeus.Services
{
	public class AnalyserHostedService : IHostedService, IDisposable
	{

		#region Properties

		private Timer _timer;

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

				//IEnumerable<MovieModel> newMovies = moviesManager.AnalysePaths().Result;
				//if(newMovies != null && newMovies.Any())
				//{
					//var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					//List<IdentityUser> listUser = db.Users.ToList();
					//SendMail(listUser, newMovies);
				//}
			}
		}


		/// <summary>
		/// Pour l'instant pas encore utilisé.
		/// </summary>
		/// <param name="listUser"></param>
		/// <param name="movies"></param>
		private void SendMail(List<IdentityUser> listUser, IEnumerable<MovieModel> movies)
		{
			try
			{
				// Pas de mail, pas d'envoie
				if (string.IsNullOrEmpty(Settings.GetMail()))
					return;

				string fromMail = Settings.GetMail();
				string pass = Settings.GetPasswordMail();

				foreach (var user in listUser)
				{
					if (user.UserName == "root")
						continue;

					string toMail = user.Email;

					MailMessage message = new MailMessage()
					{
						From = new MailAddress(fromMail),
						Subject = "Test",
						IsBodyHtml = false,
						Body = "Pour tester"
					};					
					message.To.Add(new MailAddress(toMail));

					SmtpClient smtp = new SmtpClient("smtp.office365.com", 587)
					{
						Credentials = new NetworkCredential(fromMail, pass),
						EnableSsl = false
					};
										
					smtp.Send(message);
				}
			}
			catch (Exception ex)
			{
				// Mettre un log
			}
		}

		#endregion



	}
}
