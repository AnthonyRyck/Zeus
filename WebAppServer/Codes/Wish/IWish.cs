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
        Task<IEnumerable<WishModel>> GetWishes();
    }
}
