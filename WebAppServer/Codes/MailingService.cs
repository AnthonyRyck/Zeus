using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using WebAppServer.Models;
using WepAppServer.Data;

namespace WebAppServer.Codes
{
	public class MailingService : IMailing
	{
		#region Properties

		private readonly ISettings _settings;
		private readonly ApplicationDbContext _appContext;
		private readonly string _pathVideoContentHtml;
		private readonly string _pathMailContentHtml;

		private const string MEMBER = "MEMBER";
		private const string MANAGER = "MANAGER";

		#endregion

		#region Constructeur

		public MailingService(ISettings settings, ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
		{
			_settings = settings;
			_appContext = context;
			string webRoot = hostingEnvironment.WebRootPath;

			_pathVideoContentHtml = Path.Combine(webRoot, "mail", "videoContent.html");
			_pathMailContentHtml = Path.Combine(webRoot, "mail", "mailContent.html");
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

		private string GetMessageComplet(IEnumerable<MovieModel> newVideos)
		{
			string bodyMessage = string.Empty;

			string template = File.ReadAllText(_pathVideoContentHtml);
			foreach (MovieModel video in newVideos)
			{
				string messageVideo = string.Format(template,
					"https://image.tmdb.org/t/p/w370_and_h556_bestv2" + video.MovieTmDb.PosterPath,
					video.MovieTmDb.Overview);

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
					var messageComplet = GetMessageComplet(newVideos);

					foreach (ApplicationUser user in usersList)
					{
						await SendMail(user.Email, messageComplet);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "Erreur dans l'envoie des mails pour les utilisateurs.");
			}
		}


		#endregion
		
	}
}
