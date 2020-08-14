using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorZeus.Models;

namespace BlazorZeus.Codes
{
    public interface ITheMovieDatabase
    {
        /// <summary>
        /// Retourne la liste des films qui sont sorties.
        /// </summary>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesNowPlayingAsync(int numeroPage = 1);

        /// <summary>
        /// Retourne la liste des films qui sont populaires.
        /// </summary>
        /// <param name="numeroPage"></param>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesPopularAsync(int numeroPage = 1);

        /// <summary>
        /// Récupère les informations pour 1 film donnée.
        /// </summary>
        /// <param name="idMovie"></param>
        /// <returns></returns>
        Task<MovieWishModel> GetMovie(int idMovie);

        /// <summary>
        /// Retourne la liste des films qui vont sortir au cinéma.
        /// </summary>
        /// <param name="numeroPage"></param>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesUpcomingAsync(int numeroPage = 1);

        /// <summary>
        /// Retourne la liste des films qui sont les mieux notés.
        /// </summary>
        /// <param name="numeroPage"></param>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesTopRatedAsync(int numeroPage = 1);
        
        /// <summary>
        /// Cherche les films par rapport au nom qui est donnée.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numeroPage"></param>
        /// <returns></returns>
        Task<CollectionMovieWishModel> GetMoviesByName(string name, int numeroPage = 0);
    }
}
