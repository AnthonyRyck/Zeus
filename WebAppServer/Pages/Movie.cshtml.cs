using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoviesLib.Entities;
using WebAppServer.Codes;

namespace WebAppServer.Pages
{
    public class MovieModel : PageModel
    {
        #region Fields

        private IShowsAndMovies _moviesManager;

        #endregion

        #region Properties

        /// <summary>
        /// Liste de tous les films.
        /// </summary>
        public IEnumerable<Models.MovieModel> Movies { get; set; }

        #endregion

        #region Constructeur

        public MovieModel(IShowsAndMovies movieManager)
        {
            _moviesManager = movieManager;
        }

        #endregion



        public async void OnGet()
        {
            Movies = await _moviesManager.GetMovies();
        }


        public FileResult Download(Guid id)
        {
            //var fileName = movieInformation.FileName;
            //var filepath = movieInformation.PathFile;


            var movie = Movies.FirstOrDefault(x => x.Id == id);

            byte[] fileBytes = System.IO.File.ReadAllBytes(movie.MovieInformation.PathFile);
            return File(fileBytes, "application/octet-stream", movie.MovieInformation.FileName);
        }
    }
}