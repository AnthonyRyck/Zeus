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

        public IEnumerable<WishModel> GetWishes()
        {
            return _storage.GetWishList();
        }

        public IEnumerable<WishModel> GetWishes(string idUser)
        {
            var allWishesSaved = _storage.GetWishList();
            Guid userId = Guid.Parse(idUser);

            return allWishesSaved.Where(x => x.IdUsers.Contains(userId))
                                .ToList();
        }

        /// <summary>
        /// Permet d'ajout un film dans la liste de souhait, ou ajouter
        /// un utilisateur sur le film déjà souhaité.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="idUser"></param>
        public void AddMovie(MovieWishModel movie, Guid idUser)
        {
            if (HaveMovieInWish(movie.IdVideoTmDb))
            {
                // Ajout de l'utilisateur dans la liste
                WishModel wish = _wishListModels.FirstOrDefault(x => x.Movie.IdVideoTmDb == movie.IdVideoTmDb);

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
                WishModel model = new WishModel(movie, new List<Guid>(){idUser});
                _wishListModels.Add(model);
                _storage.SaveWishModels(_wishListModels);
            }
        }

        /// <summary>
        /// Permet de supprimer un utilisateur d'un film souhaité,
        /// et si plus aucun utilisateur, enlever le film de la liste.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="idUser"></param>
        public void RemoveMovie(MovieWishModel movie, Guid idUser)
        {
            if (HaveMovieInWish(movie.IdVideoTmDb))
            {
                // Ajout de l'utilisateur dans la liste
                WishModel wish = _wishListModels.FirstOrDefault(x => x.Movie.IdVideoTmDb == movie.IdVideoTmDb);

                if (wish != null)
                {
                    if (wish.HasUserId(idUser))
                    {
                        wish.IdUsers.Remove(idUser);

                        if (wish.IdUsers.Count == 0)
                        {
                            _wishListModels.Remove(wish);
                        }

                        _storage.SaveWishModels(_wishListModels);
                    }
                }
            }
        }

        /// <inheritdoc cref="IWish.HaveMovieInWish"/>
        public bool HaveMovieInWish(int idMovie, string idUser)
        {
            return _wishListModels.Any(x => x.Movie.IdVideoTmDb == idMovie 
                                        && x.IdUsers.Contains(Guid.Parse(idUser)));
        }

        #endregion

        #region Private Methods

        private bool HaveMovieInWish(int idMovie)
        {
            return _wishListModels.Any(x => x.Movie.IdVideoTmDb == idMovie);
        }

        #endregion
    }
}
