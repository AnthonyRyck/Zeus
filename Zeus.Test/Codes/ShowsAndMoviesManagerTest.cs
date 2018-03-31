using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zeus.Codes;

namespace Zeus.Test.Codes
{
    [TestClass]
    public class ShowsAndMoviesManagerTest
    {
        [TestMethod]
        public void RecuperationDeFilms_When_MovieFolderHas2Films_Then_2Films()
        {
            #region Arrange

            string pathMovie = "/movies";

            if (!Directory.Exists(pathMovie))
            {
                Directory.CreateDirectory(pathMovie);
            }

            // Création de faux films.
            DirectoryInfo directoryMovie = new DirectoryInfo(pathMovie);

            File.Create(directoryMovie.FullName + @"\An.American.Haunting.2005.TRUEFRENCH.DVDRip.XviD.Mp3-Tetine.avi");
            File.Create(directoryMovie.FullName + @"\paddington-2-french-bluray-1080p-2018.mkv");
            File.Create(directoryMovie.FullName + @"\mazinger-z-french-bluray-1080p-2018.mp4");
            File.Create(directoryMovie.FullName + @"\fichierWeb.html");
            File.Create(directoryMovie.FullName + @"\readme.txt");

            #endregion

            #region Act

            ShowsAndMoviesManager manager = new ShowsAndMoviesManager();
            var result = manager.GetListMoviesLocal();

            #endregion

            #region Assert

            Assert.AreEqual(3, result.Count(), "Doit contenir 3 films.");

            #endregion
        }
    }
}
