﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class LogFile
    {
	    #region Properties

		/// <summary>
		/// Chemin complet du fichier de log, avec son nom
		/// </summary>
	    public string FullPath { get; set; }

		/// <summary>
		/// Date du fichier log.
		/// </summary>
	    public DateTime DateFile { get; set; }

		/// <summary>
		/// Retourne la date sous forme : 04/07/2018
		/// </summary>
		public String DateEnLettre => DateFile.ToString("d");

	    #endregion

	    #region Constructeur

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="pathFile">Chemin d'accès avec le nom du fichier de log.</param>
	    public LogFile(string pathFile)
	    {
		    FullPath = pathFile;
		    DateFile = GetDateFile(pathFile);
	    }

	    #endregion

	    #region Private Methods

		/// <summary>
		/// Extrait la date du fichier de log.
		/// </summary>
		/// <param name="pathFile">Chemin d'accès avec le nom du fichier.</param>
		/// <returns></returns>
	    private DateTime GetDateFile(string pathFile)
		{
			string[] splitTemp = pathFile.Split("log-", StringSplitOptions.RemoveEmptyEntries);
			string[] dateSplit = splitTemp[1].Split(".txt", StringSplitOptions.RemoveEmptyEntries);

			DateTime theTime = DateTime.ParseExact(dateSplit[0],
				"yyyyMMdd",
				CultureInfo.InvariantCulture,
				DateTimeStyles.None);

			return theTime;
		}

	    #endregion
    }
}
