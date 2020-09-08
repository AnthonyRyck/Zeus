using BlazorZeus.Codes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Composants
{
	public partial class ConfigApplication
	{
		#region Properties

		[Inject]
		public ISettings Settings { get; set; }

		public ConfigModel ConfigModelApp { get; set; }

		#endregion

		protected override void OnInitialized()
		{
			ConfigModelApp = new ConfigModel()
			{
				ListeDesLangues = Settings.GetLanguesVideos().ToList(),
				LanguesPourTmDb = Settings.GetLangueTmdb(),
				RegionPourTmdb = Settings.GetRegionTmdb(),
				PathMovies = Settings.GetPathMovies().ToList(),
				Email = Settings.GetMail(),
				PasswordEmail = Settings.GetPasswordMail(),
				TimeToRefresh = Settings.GetTimeToUpdateVideos()
			};
		}



		/// <summary>
		/// Méthode levé quand le model est validé.
		/// </summary>
		protected void HandleValidSubmit()
		{
			if(ConfigModelApp.TimeToRefresh < 5)
			{
				ConfigModelApp.TimeToRefresh = 5;
				StateHasChanged();
				return;
			}

			// Sauvegarde
			Settings.SaveSettings(ConfigModelApp.LanguesPourTmDb,
									ConfigModelApp.RegionPourTmdb,
									ConfigModelApp.TimeToRefresh,
									ConfigModelApp.Email,
									ConfigModelApp.PasswordEmail);
		}


	}




	#region Class Model

	// *********** Exemple de fichier de configuration ***********
	//
	// {"ListeDeLangue":["FRENCH","TRUEFRENCH","FR"],
	// "LanguePourTmDb":"fr-FR","RegionPourTmDb":"FR",
	// "PathMovies":["C:/Docker/Zeus/movies"],
	// "PathDessinAnimes":["C:/Docker/Zeus/animes"],
	// "PathShows":["C:/Docker/Zeus/series"],
	// "TempsEnMillisecondPourTimerRefresh":60000},
	// "Email":"plouf@gmail.com",
	// "PasswordMail":"okokokok"

	public class ConfigModel
	{
		public List<string> ListeDesLangues { get; set; }

		public string LanguesPourTmDb { get; set; }

		public string RegionPourTmdb { get; set; }

		public List<string> PathMovies { get; set; }

		public List<string> PathSeries { get; set; }

		public List<string> PathDessinAnimes { get; set; }

		public string Email { get; set; }

		public string PasswordEmail { get; set; }

		[Required]
		public uint TimeToRefresh { get; set; }
	}

	#endregion
}
