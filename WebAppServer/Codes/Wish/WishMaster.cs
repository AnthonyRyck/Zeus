using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
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
            return _storage.GetWishList();
        }

        public void AddMovie(int idMovie, Guid idUser)
        {
            if (HaveMovieInWish(idMovie))
            {
                // Ajout de l'utilisateur dans la liste
                WishModel wish = _wishListModels.FirstOrDefault(x => x.IdMovie == idMovie);

                if (wish != null)
                {
                    if (!wish.HasUserId(idUser))
                    {
                        wish.IdUsers.Add(idUser);

                        _storage.SaveWishModels(_wishListModels);
                    }
                }
            }
            else
            {
                // Ajout du film dans la liste de Souhait.
                WishModel model = new WishModel(idMovie, new List<Guid>(){idUser});
                _wishListModels.Add(model);
                _storage.SaveWishModels(_wishListModels);
            }
        }

        #endregion

        #region Private Methods

        private bool HaveMovieInWish(int idMovie)
        {
            return _wishListModels.Any(x => x.IdMovie == idMovie);
        }

        #endregion
    }
}
