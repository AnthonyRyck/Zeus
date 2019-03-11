using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Codes.Wish
{
    public class WishMaster : IWish 
    {
        #region Properties

        /// <summary>
        /// Contient la liste de tous les souhaits en film
        /// </summary>
        private List<WishModel> _wishListModels = null;
        private readonly StorageManager _storage;

        #endregion


        #region Constructeur

        public WishMaster()
        {
            _storage = new StorageManager();

            var tempWishList = _storage.GetWishList();

            _wishListModels = tempWishList != null 
                ? tempWishList.ToList() 
                : new List<WishModel>();
        }

        #endregion

        #region Implement IWish

        public async Task<IEnumerable<WishModel>> GetWishes()
        {
            // Faire la jointure entre fichier wish et les utilidateurs.

            return null;
        }

        #endregion

    }
}
