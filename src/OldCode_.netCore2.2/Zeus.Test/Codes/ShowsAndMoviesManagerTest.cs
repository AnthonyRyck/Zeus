﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAppServer.Codes;

namespace Zeus.Test.Codes
{
    [TestClass]
    public class ShowsAndMoviesManagerTest
    {
        private const string PATH_MOVIE = "/movies";


        [TestCleanup]
        public void TestInitialize()
        {
            // Suppression des fichiers de tests (movies, shows,...)
            DirectoryInfo directoryMovie = new DirectoryInfo(PATH_MOVIE);
            foreach (FileInfo file in directoryMovie.GetFiles())
            {
                file.Delete();
            }

            // Suppression des fichiers de configuration créé.
            DirectoryInfo directoryApp = new DirectoryInfo(AppContext.BaseDirectory + "/save");
            if (directoryApp.Exists)
            {
                directoryApp.Delete(true);
            }

            DirectoryInfo directoryConfig = new DirectoryInfo(AppContext.BaseDirectory + "/config");
            if (directoryConfig.Exists)
            {
                directoryConfig.Delete(true);
            }

            DirectoryInfo directorySave = new DirectoryInfo(AppContext.BaseDirectory + "/save");
            if (directorySave.Exists)
            {
                directorySave.Delete(true);
            }
        }

        #region Method GetListMoviesLocal
        
        [TestMethod]
        public async void RecuperationDeFilms_When_MovieFolderIsEmpty_Then_NoFilm()
        {
            #region Act

            MoviesManager manager = new MoviesManager();
            var result = await manager.GetListMoviesLocal();

            #endregion

            #region Assert

            Assert.AreEqual(0, result.Count(), "Doit contenir aucun film.");

            #endregion
        }


        [TestMethod]
        public async void RecuperationDeFilms_When_MovieFolderHas3Films_Then_3Films()
        {
            #region Arrange

            if (!Directory.Exists(PATH_MOVIE))
            {
                Directory.CreateDirectory(PATH_MOVIE);
            }

            // Création de faux films.
            DirectoryInfo directoryMovie = new DirectoryInfo(PATH_MOVIE);

            var fsAmercian = File.Create(directoryMovie.FullName + @"\An.American.Haunting.2005.TRUEFRENCH.DVDRip.XviD.Mp3-Tetine.avi");
            var movie2 = File.Create(directoryMovie.FullName + @"\paddington-2-french-bluray-1080p-2018.mkv");
            var movie3 = File.Create(directoryMovie.FullName + @"\mazinger-z-french-bluray-1080p-2018.mp4");
            var other1 = File.Create(directoryMovie.FullName + @"\fichierWeb.html");
            var other2 = File.Create(directoryMovie.FullName + @"\readme.txt");

            fsAmercian.Dispose();
            movie2.Dispose();
            movie3.Dispose();
            other2.Dispose();
            other1.Dispose();

            #endregion

            #region Act

            MoviesManager manager = new MoviesManager();
            var result = await manager.GetListMoviesLocal();

            #endregion

            #region Assert

            Assert.AreEqual(3, result.Count(), "Doit contenir 3 films.");

            #endregion
        }

        #endregion

        #region Method GetMovies

        [TestMethod]
        public void RecuperationDeFilmsAvecTmDb_When_MovieFolderHas3Films_Then_3Films()
        {
            #region Arrange

            if (!Directory.Exists(PATH_MOVIE))
            {
                Directory.CreateDirectory(PATH_MOVIE);
            }

            // Création de faux films.
            DirectoryInfo directoryMovie = new DirectoryInfo(PATH_MOVIE);

            var fsAmercian = File.Create(directoryMovie.FullName + @"\An.American.Haunting.2005.TRUEFRENCH.DVDRip.XviD.Mp3-Tetine.avi");
            var movie2 = File.Create(directoryMovie.FullName + @"\paddington-2-french-bluray-1080p-2018.mkv");
            var movie3 = File.Create(directoryMovie.FullName + @"\mazinger-z-french-bluray-1080p-2018.mp4");
            var other1 = File.Create(directoryMovie.FullName + @"\fichierWeb.html");
            var other2 = File.Create(directoryMovie.FullName + @"\readme.txt");

            fsAmercian.Dispose();
            movie2.Dispose();
            movie3.Dispose();
            other2.Dispose();
            other1.Dispose();

            #endregion

            #region Act

            MoviesManager manager = new MoviesManager();
            var result = manager.GetMovies().Result;

            #endregion

            #region Assert

            Assert.AreEqual(3, result.Count(), "Doit contenir 3 films.");

            DirectoryInfo directorySave = new DirectoryInfo(AppContext.BaseDirectory + "/save");
            var files = directorySave.GetFiles();

            Assert.AreEqual(2, files.Count(), "Il doit y avoir 2 fichiers de sauvegarde");

            #endregion
        }

        #endregion

    }
}
