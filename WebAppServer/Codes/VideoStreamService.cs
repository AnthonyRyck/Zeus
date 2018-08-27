using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
	public class VideoStreamService : IVideoStreamService
	{
		private readonly IMovies _moviesManager;


		public VideoStreamService(IMovies moviesManager)
		{
			_moviesManager = moviesManager;
		}


		/// <inheritdoc />
		public FileStreamResult GetVideoById(Guid idVideo)
		{
			MovieModel movie = _moviesManager.GetMovie(idVideo);

			var result = new FileStream(movie.MovieInformation.PathFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
				true);

			return new FileStreamResult(result, "video/mp4");
		}
	}
}
