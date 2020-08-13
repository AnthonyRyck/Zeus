using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using TMDbLib.Objects.TvShows;
using WebAppServer.Codes.Wish;
using WebAppServer.Models;
using WepAppServer.Data;

namespace WebAppServer.Codes
{
	public class MailingService : IMailing
	{
		#region Properties

		private readonly ISettings _settings;
		private readonly ApplicationDbContext _appContext;
	    private readonly IWish _wishManager;

        private readonly string _pathVideoContentHtml;
		private readonly string _pathMailContentHtml;
		private readonly string _pathSerieContentHtml;
	    private readonly string _pathWishMovieContentHtml;

		private const string MEMBER = "MEMBER";
		private const string MANAGER = "MANAGER";

		#endregion

		#region Constructeur

		public MailingService(ISettings settings, ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
		    IWish wishManager)
		{
			_settings = settings;
			_appContext = context;
			string webRoot = hostingEnvironment.WebRootPath;
		    _wishManager = wishManager;

            _pathVideoContentHtml = Path.Combine(webRoot, "mail", "videoContent.html");
			_pathMailContentHtml = Path.Combine(webRoot, "mail", "mailContent.html");
			_pathSerieContentHtml = Path.Combine(webRoot, "mail", "serieContent.html");
		    _pathWishMovieContentHtml = Path.Combine(webRoot, "mail", "wishMovieContent.html");

		}
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Permet de récupérer tous les utilisateurs qui ont un rôle de membre
		/// ou de manager.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ApplicationUser> GetAllUsersExceptAdmin()
		{
			List<ApplicationUser> usersList = new List<ApplicationUser>();

			try
			{
				// Récupération des ids pour les rôles de membre et manager.
				IEnumerable<string> idsRoles = _appContext.Roles.Where(x => x.NormalizedName == MANAGER
																			|| x.NormalizedName == MEMBER)
					.Select(x => x.Id)
					.ToList();

				// Récupération des utilisateurs ayant pour ces rôles.
				IEnumerable<string> idUserRole = _appContext.UserRoles.Where(x => idsRoles.Contains(x.RoleId))
					.Select(x => x.UserId)
					.ToList();

				// Récupération des utilisateurs.
				usersList = _appContext.Users.Where(x => idUserRole.Contains(x.Id)).ToList();
			}
			catch (Exception exception)
			{
				Log.Error(exception, "Erreur dans la récupération de la liste des utilisateurs Membre et Manager.");
			}

			return usersList;
		}


		private async Task SendMail(string mailTo, string messageBody)
		{
			using (var smtpClient = new SmtpClient
										{
											Host = "smtp.gmail.com",
											Port = 587,
											EnableSsl = true,
											Credentials = new NetworkCredential(_settings.GetMail(), _settings.GetPasswordMail())
			})
			{
				using (var message = new MailMessage(_settings.GetMail(), mailTo)
				{
					Subject = "Zeus t'informe !",
					Body = messageBody,
					IsBodyHtml = true
				})
				{
					await smtpClient.SendMailAsync(message);
				}
			}
		}

        /// <summary>
        /// Méthode qui va créer le mail sous format HTML.
        /// </summary>
        /// <param name="newVideos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
		private string GetMessageComplet(IEnumerable<MovieModel> newVideos, string userId)
		{
			string bodyMessage = string.Empty;

			string template = File.ReadAllText(_pathVideoContentHtml);
		    string templateWish = File.ReadAllText(_pathWishMovieContentHtml);

			foreach (MovieModel video in newVideos)
			{
			    string messageVideo;

                // Dans le cas ou la vidéo n'est pas trouvé sur le site de TmDb.
                if (video.MovieTmDb.Id == 0)
                {
                    messageVideo = string.Format(template,
                        "https://image.tmdb.org/t/p/w370_and_h556_bestv2",
                        "Titre non trouvé. Titre extrait du nom du fichier : " + video.MovieInformation.Titre);
                }
			    else
                {
                    // Dans le cas ou la vidéo est dans la WishList.
                    if (_wishManager.HaveMovieInWish(video.MovieTmDb.Id, userId))
                    {
                        messageVideo = string.Format(templateWish,
                            "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + video.MovieTmDb.PosterPath, video.MovieTmDb.Overview);

                        // Suppression du film de la WishList
                        _wishManager.RemoveMovie(video.MovieTmDb.Id, userId);
                    }
                    else
                    {
                        messageVideo = string.Format(template,
                            "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + video.MovieTmDb.PosterPath, video.MovieTmDb.Overview);
                    }
                }
                
				bodyMessage += messageVideo;
			}

			string message = File.ReadAllText(_pathMailContentHtml);
			return string.Format(message, bodyMessage);
		}

		private string GetMessageComplet(IEnumerable<KeyValuePair<TvSeason, TvEpisode>> nouveauteSeries)
		{
			string bodyMessage = string.Empty;

			string template = File.ReadAllText(_pathSerieContentHtml);
			foreach (KeyValuePair<TvSeason, TvEpisode> video in nouveauteSeries)
			{
				string messageVideo = string.Format(template,
					"https://image.tmdb.org/t/p/w370_and_h556_bestv2" + video.Key.PosterPath,
					video.Value.Overview, video.Key.SeasonNumber, video.Value.EpisodeNumber);

				bodyMessage += messageVideo;
			}

			string message = File.ReadAllText(_pathMailContentHtml);
			return string.Format(message, bodyMessage);
		}

		#endregion

		#region Implement IMailing

		/// <inheritdoc />
		public async Task SendNewVideo(IEnumerable<MovieModel> newVideos)
		{
			try
			{
				IEnumerable<ApplicationUser> usersList = GetAllUsersExceptAdmin();

				if (newVideos.Any())
				{
                    foreach (ApplicationUser user in usersList)
					{
                        var messageComplet = GetMessageComplet(newVideos, user.Id);
                        await SendMail(user.Email, messageComplet);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "Erreur dans l'envoie des mails pour les films.");
			}
		}

		/// <inheritdoc />
		public async Task SendNewVideo(IEnumerable<KeyValuePair<TvSeason, TvEpisode>> nouveauteSeries)
		{
			try
			{
				IEnumerable<ApplicationUser> usersList = GetAllUsersExceptAdmin();

				if (nouveauteSeries.Any())
				{
					var messageComplet = GetMessageComplet(nouveauteSeries);

					foreach (ApplicationUser user in usersList)
					{
						await SendMail(user.Email, messageComplet);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "Erreur dans l'envoie des mails pour les séries.");
			}
		}

		#endregion
		
	}
}
