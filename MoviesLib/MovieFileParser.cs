using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MoviesLib.Entities;

namespace MoviesLib
{
    /// <summary>
    /// Class permettant de faire l'extraction des informations du titre
    /// d'un film (nom de fichier).
    /// </summary>
    public class MovieFileParser
    {
        #region Properties

        private const string RESOLUTION_PATTERN = "[0-9]{3,4}p";
        private const string QUALITY_PATTERN = @"([HP]DTV|HDCAM|B[rR]Rip|TS|WEB-DL|H[dD]Rip|DVDRip|DVDRiP|DVDRIP|CamRip|webrip|W[EB]B[rR]ip|[Bb]lu[Rr]ay|DvDScr|hdtv|[Hh][Dd][Ll]ight)";
        private const string INCONNU = "Inconnu";

        private string _languesPattern;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur du Parser pour les noms des Torrents.
        /// </summary>
        /// <param name="langues">Liste des langues que l'utilisateur veut r�cup�rer.</param>
        public MovieFileParser(params string[] langues)
        {
            _languesPattern = BuildPattern(langues);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Permet de r�cup�rer les informations sur un film par rapport
        /// � son titre.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public MovieInformation GetInformation(string title)
        {
            MovieInformation result = new MovieInformation();

            // Ajout de "." � la place des espaces.
            title = title.Replace(' ', '.')
                         .Replace('-','.');

            result.Resolution = GetResolution(ref title, RESOLUTION_PATTERN);
            result.Qualite = GetQuality(ref title, QUALITY_PATTERN);
            result.Annee = GetAnneeProduction(ref title);
            result.Langage = GetLanguage(ref title);
            result.Titre = GetTitle(title);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// R�cup�re la r�soluion de la vid�o.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string GetResolution(ref string title, string pattern)
        {
            Match matchResult = Regex.Match(title, pattern);
            title = RemoveResultRegex(matchResult.Value, title);

            return string.IsNullOrEmpty(matchResult.Value)
                ? INCONNU
                : matchResult.Value;
        }

        /// <summary>
        /// R�cup�re la qualit�.
        /// </summary>
        /// <param name="title">Titre du film.</param>
        /// <param name="pattern">Pattern du Regex.</param>
        /// <returns></returns>
        private string GetQuality(ref string title, string pattern)
        {
            var matchResult = Regex.Match(title, pattern);
            title = RemoveResultRegex(matchResult.Value, title);

            return string.IsNullOrEmpty(matchResult.Value)
                ? INCONNU
                : matchResult.Value.Replace(".", String.Empty);
        }

        /// <summary>
        /// Supprime du titre ce qui est trouv� dans le Regex.
        /// </summary>
        /// <param name="resultatRegex"></param>
        /// <param name="titre"></param>
        /// <returns></returns>
        private string RemoveResultRegex(string resultatRegex, string titre)
        {
            return !string.IsNullOrEmpty(resultatRegex)
                ? titre.Replace(resultatRegex, string.Empty)
                : titre;
        }

        /// <summary>
        /// Extrait le titre de la serie/film.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string GetTitle(string title)
        {
            int indexDoublePoint = title.IndexOf("..");
            string result;

            if (indexDoublePoint > -1)
            {
                string tempTitle = title.Substring(0, indexDoublePoint);
                string retourTitle = tempTitle.Replace('.', ' ');

                // J'enl�ve tous caract�res qui n'est pas alphab�tique.
                // \W tous les caract�res qui ne sont pas alphanum�rique
                // ^\s exception sur les espaces.
                result = Regex.Replace(retourTitle, @"\W^\s", string.Empty);
            }
            else
            {
                result = title;
            }

            return result;
        }

        /// <summary>
        /// Extrait l'ann�e de production d'un film.
        /// DOIT PASSER APRES l'extraction de la r�solution.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string GetAnneeProduction(ref string title)
        {
            const string patternAnnee = "[0-9]{4}";

            Match matchResult = Regex.Match(title, patternAnnee);
            title = RemoveResultRegex(matchResult.Value, title);

            return string.IsNullOrEmpty(matchResult.Value)
                ? INCONNU
                : matchResult.Value;
        }

        /// <summary>
        /// R�cup�re le language de la vid�o. La langue cherch� est donn�e en 
        /// param�tre dans le constructeur.
        /// </summary>
        /// <returns></returns>
        private string GetLanguage(ref string title)
        {
            string titleTempMajuscule = title.ToUpper();
            Match matchResult = Regex.Match(titleTempMajuscule, _languesPattern);

            title = RemoveResultRegex(matchResult.Value, titleTempMajuscule);

            return string.IsNullOrEmpty(matchResult.Value)
                ? INCONNU
                : matchResult.Value.Replace(".", string.Empty);
        }

        /// <summary>
        /// Construit un pattern pour un Regex.
        /// </summary>
        /// <param name="langues"></param>
        /// <returns></returns>
        private string BuildPattern(string[] langues)
        {
            // Il faut que le pattern commence par '.'
            string result = @"(";

            for (int i = 0; i < langues.Length; i++)
            {
                if (i == langues.Length - 1)
                    result += langues[i].ToUpper();
                else
                    result += langues[i].ToUpper() + "|"; // '|' signe 'ou' pour le pattern.
            }

            // et finisse par un '.'
            result += @")";

            return result;
        }

        #endregion
    }
}