using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesLib.Entities
{
	/// <summary>
	/// Class contenant les informations d'une série.
	/// </summary>
    public class ShowInformation : VideoInformation
    {
	    #region Properties

		/// <summary>
		/// Numéro de la saison.
		/// </summary>
	    public short Saison { get; set; }

		/// <summary>
		/// Numéro de l'épisode.
		/// </summary>
	    public short Episode { get; set; }

		#endregion
	}
}
