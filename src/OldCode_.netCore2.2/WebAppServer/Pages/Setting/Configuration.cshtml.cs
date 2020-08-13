using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppServer.Codes;

namespace WebAppServer.Pages.Setting
{
    public class ConfigurationModel : PageModel
    {
		#region Properties
		

		[BindProperty]
		public ConfigModel Input { get; set; }

		/// <summary>
		/// Affichage du filtre pour les langues voulues
		/// </summary>
	    public string FiltreLanguesPourLesVideos { get; set; }


	    private readonly ISettings _setting;

		#endregion

	    #region Constructeur

	    public ConfigurationModel(ISettings settings)
	    {
		    _setting = settings;
	    }

	    #endregion

		#region Methods Razor

		public void OnGet()
		{
			Input = new ConfigModel()
			{
				LanguesPourTmDb = _setting.GetLangueTmdb(),
				RegionPourTmdb = _setting.GetRegionTmdb(),
				TimeToRefresh = MillisecondToMinute(_setting.GetTimeToUpdateVideos()), // passage en minute.
				Email = _setting.GetMail(),
				PasswordEmail = _setting.GetPasswordMail()
			};
		}
		
	    public async Task<IActionResult> OnPostAsync()
	    {
			// Sauvegarde de la configuration
		    await _setting.SaveSettings(langueTmdb: Input.LanguesPourTmDb, 
										regionTmdb: Input.RegionPourTmdb, 
										tempsRefresh: MinuteToMillisecond(Input.TimeToRefresh),
										email:Input.Email,
										passwordMail:Input.PasswordEmail);

		    return Page();
	    }


		#endregion

	    #region Private Methods

		/// <summary>
		/// Convertit un temps de millisecondes en minutes.
		/// </summary>
		/// <param name="millisecond"></param>
		/// <returns></returns>
	    private static int MillisecondToMinute(int millisecond)
	    {
		    return millisecond / 1000 / 60;
	    }

		/// <summary>
		/// Convertit un temps de minute en millisecondes
		/// </summary>
		/// <param name="minute"></param>
		/// <returns></returns>
	    private static int MinuteToMillisecond(int minute)
	    {
		    return minute * 60 * 1000;
	    }

	    #endregion

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
		    [DisplayName("Liste des langues")]
			public List<string> ListeDesLangues { get; set; }
			
		    [DisplayName("Langue pour The Movie Database")]
			public string LanguesPourTmDb { get; set; }

			[DisplayName("Region utilise pour The Movie Database")]
			public string RegionPourTmdb { get; set; }
			
			[DisplayName("Chemin d'acces pour les films")]
			public List<string> PathMovies { get; set; }

			[DisplayName("Chemin d'acces pour les series")]
		    public List<string> PathSeries { get; set; }

			[DisplayName("Chemin pour les dessins animes")]
		    public List<string> PathDessinAnimes { get; set; }

			[DisplayName("Email utilisé pour l'envoie de mail (pour l'instant QUE un gmail")]
			public string Email { get; set; }

			[DisplayName("Mot de passe du compte mail")]
			public string PasswordEmail { get; set; }

			[Required]
			[DisplayName("Temps pour rafraichissement")]
		    public int TimeToRefresh { get; set; }
	    }

		#endregion
		
	}
}