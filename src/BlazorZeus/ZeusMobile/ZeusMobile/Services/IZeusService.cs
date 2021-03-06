using MoviesLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeusCore;

namespace ZeusMobile.Services
{
	public interface IZeusService
	{
		Task<List<InformationMovie>> GetAllMovies();
		Task<DetailMovie> GetMovie(Guid idMovie);
	}
}
