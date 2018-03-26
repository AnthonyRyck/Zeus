using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesLib.Entities
{
	/// <summary>
	/// Class contenant les informations d'une série.
	/// </summary>
    public class ShowInformation
    {
	    #region Properties

		/// <summary>
		/// Titre de la série
		/// </summary>
	    public string Titre { get; set; }

		/// <summary>
		/// Année de production
		/// </summary>
	    public string Annee { get; set; }

		/// <summary>
		/// Numéro de la saison.
		/// </summary>
	    public short Saison { get; set; }

		/// <summary>
		/// Numéro de l'épisode.
		/// </summary>
	    public short Episode { get; set; }

		/// <summary>
		/// Résolution de l'épisode (1080p, 720p,...).
		/// </summary>
	    public string Resolution { get; set; }

		/// <summary>
		/// Qualité de l'épisode (HDTV, LD,...)
		/// </summary>
		public string Qualite { get; set; }

		/// <summary>
		/// Langue de l'épisode.
		/// </summary>
	    public string Langage { get; set; }

		#endregion
	}
}
