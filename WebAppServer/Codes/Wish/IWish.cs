using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Codes.Wish
{
    public interface IWish
    {
        /// <summary>
        /// Permet de récupérer toutes la liste de souhait.
        /// </summary>
        /// <returns></returns>
        IEnumerable<WishModel> GetWishes();

        /// <summary>
        /// Permet de récupérer toutes la liste de souhait pour un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<WishModel> GetWishes(string userId);


        /// <summary>
        /// Permet d'ajouter un film dans la liste de souhait.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="idUser"></param>
        void AddMovie(MovieWishModel movie, Guid idUser);

        
    }
}
