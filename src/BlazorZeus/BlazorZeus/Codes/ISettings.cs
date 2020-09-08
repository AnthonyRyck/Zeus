using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Codes
{
    public interface ISettings
    {
		/// <summary>
	    /// Retourne la liste des langues choisies. ("FRENCH", "TRUEFRENCH", "FR")
	    /// </summary>
	    /// <returns></returns>
	    IEnumerable<string> GetLanguesVideos();

	    /// <summary>
	    /// Donne le temps pour faire la mise à jour des vidéos présentes sur le serveur.
	    /// Temps en secondes.
	    /// </summary>
	    /// <returns></returns>
	    uint GetTimeToUpdateVideos();

	    /// <summary>
	    /// Retourne la liste des chemins pour les vidéos des films.
	    /// </summary>
	    /// <returns></returns>
	    IEnumerable<string> GetPathMovies();

	    /// <summary>
	    /// Retourne la liste des chemins pour les vidéos des dessins animés.
	    /// </summary>
	    /// <returns></returns>
	    IEnumerable<string> GetPathDessinAnimes();

		/// <summary>
		/// Retourne la liste des chemins pour les vidéos des séries.
		/// </summary>
		/// <returns></returns>
	    IEnumerable<string> GetPathShows();

		/// <summary>
		/// Retourne la langue choisie pour les informations de TmDb.
		/// </summary>
		/// <returns></returns>
	    string GetLangueTmdb();

	    /// <summary>
	    /// Retourne la région pour les informations de Tmdb
	    /// </summary>
	    /// <returns></returns>
	    string GetRegionTmdb();

		/// <summary>
		/// Permet de faire la sauvegarde des éléments pour la configuration.
		/// </summary>
		/// <returns></returns>
	    Task SaveSettings(string langueTmdb = "fr-FR", string regionTmdb = "FR",
			uint tempsRefresh = 3600000, string email = "", string passwordMail = "");
		
		/// <summary>
		/// Retourne l'adresse mail pour l'envoie d'email.
		/// </summary>
		/// <returns></returns>
		string GetMail();

		/// <summary>
		/// Mot de passe pour le compte mail.
		/// </summary>
		/// <returns></returns>
		string GetPasswordMail();
	}
}
