using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesLib.Entities;

namespace MoviesLib.Test
{
    [TestClass]
    public class MovieFileParserTest
    {
        [TestMethod]
        [TestProperty("MoviesLib", "MovieFilePaser")]
        [Description("Test sur le type de retour de la méthode, doit être de type MovieInformation.")]
        public void TestSurLeRetourneDuType_When_RetourEgalMovieInformation_Then_True()
        {
            #region ARRANGE

            string titre = "paddington-2-french-bluray-1080p-2018";

            #endregion

            #region ACT

            MovieFileParser parser = new MovieFileParser();
            var resultParser = parser.GetInformation(titre, String.Empty, 0);

            #endregion

            #region ASSERT

            Assert.IsTrue(resultParser.GetType().Name == typeof(MovieInformation).Name);

            #endregion
        }

        #region Tests sur la Qualité

        [TestMethod]
        [TestProperty("MoviesLib", "MovieFileParser")]
        [Description("Il faut extraire la résolution du titre.")]
        public void ExtraireQualiteDuTitre_When_ResolutionEst1080p_Then_1080p()
        {
            #region ARRANGE

            string titre = "paddington-2-french-bluray-1080p-2018";

            #endregion

            #region ACT

            MovieFileParser parser = new MovieFileParser();
            var resultParser = parser.GetInformation(titre, String.Empty, 0);

            #endregion

            #region ASSERT

            Assert.AreEqual("1080p", resultParser.Resolution);

            #endregion
        }

        #endregion

        [TestMethod]
        [TestProperty("MoviesLib", "MovieFileParser")]
        [Description("Il faut extraire la qualité du titre.")]
        public void ExtraireLangueDuTitre_When_French()
        {
            #region ARRANGE

            string titre = "paddington-2-french-bluray-1080p-2018";

            #endregion

            #region ACT

            MovieFileParser parser = new MovieFileParser("french", "TRUEFRENCH");
            var resultParser = parser.GetInformation(titre, String.Empty, 0);

            #endregion

            #region ASSERT

            Assert.AreEqual("1080p", resultParser.Resolution);
            Assert.AreEqual("FRENCH", resultParser.Langage);

            #endregion
        }

        [TestMethod]
        [TestProperty("MoviesLib", "MovieFileParser")]
        [Description("")]
        public void ExtraireDeToutesLesInformations()
        {
            #region ARRANGE

            string titreSeries = "An.American.Haunting.2005.TRUEFRENCH.DVDRip.XviD.Mp3-Tetine";

            #endregion

            #region ACT

            MovieFileParser parser = new MovieFileParser("VOSTFR", "french", "TRUEFRENCH");
            var resultParser = parser.GetInformation(titreSeries, String.Empty, 0);

            #endregion

            #region ASSERT

            Assert.AreEqual("Inconnu", resultParser.Resolution);
            Assert.AreEqual("DVDRip", resultParser.Qualite);
            Assert.AreEqual("2005", resultParser.Annee);
            Assert.AreEqual("TRUEFRENCH", resultParser.Langage);
            Assert.AreEqual("AN AMERICAN HAUNTING", resultParser.Titre);

            #endregion
        }
        //Black Christmas 2006 TRUEFRENCH DVDRIP Xvid MP3.avi 

        [TestMethod]
        [TestProperty("MoviesLib", "MovieFileParser")]
        [Description("")]
        public void ExtraireDeToutesLesInfosDuTitre()
        {
            #region ARRANGE

            string titreSeries = "Black Christmas 2006 TRUEFRENCH DVDRIP Xvid MP3";

            #endregion

            #region ACT

            MovieFileParser parser = new MovieFileParser("VOSTFR", "french", "TRUEFRENCH");
            var resultParser = parser.GetInformation(titreSeries, String.Empty, 0);

            #endregion

            #region ASSERT

            Assert.AreEqual("Inconnu", resultParser.Resolution);
            Assert.AreEqual("DVDRIP", resultParser.Qualite);
            Assert.AreEqual("2006", resultParser.Annee);
            Assert.AreEqual("TRUEFRENCH", resultParser.Langage);
            Assert.AreEqual("BLACK CHRISTMAS", resultParser.Titre);

            #endregion
        }

    }
}
