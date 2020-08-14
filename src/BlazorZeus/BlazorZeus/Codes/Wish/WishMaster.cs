using System;
using System.Collections.Generic;
using System.Linq;
using BlazorZeus.Models;

namespace BlazorZeus.Codes.Wish
{
    public class WishMaster : IWish 
    {
        #region Properties

        private readonly StorageManager _storage = new StorageManager();
        //private readonly Timer _timerUpdate;

        /// <summary>
        /// Contient la liste de tous les souhaits en film
        /// </summary>
        private List<WishModel> _wishListModels = null;

        //protected volatile Object Lock = new object();

        #endregion

        #region Constructeur

        public WishMaster()
        {
            var tempWishList = GetWishes();

            _wishListModels = tempWishList != null 
                ? tempWishList.ToList() 
                : new List<WishModel>();

            //_timerUpdate = new Timer(TimerUpdate, null, 5000, settings.GetTimeToUpdateVideos());
        }
        
        #endregion

        #region Implement IWish


        public IEnumerable<WishModel> GetWishes()
        {
            return _storage.GetWishList();
        }

        public IEnumerable<WishModel> GetWishes(string idUser)
        {
            var allWishesSaved = GetWishes();
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

        /// <inheritdoc />
        public void RemoveMovie(int idMovie, string userId)
        {
            if(HaveMovieInWish(idMovie, userId))
            {
                WishModel wish = _wishListModels.FirstOrDefault(x => x.Movie.IdVideoTmDb == idMovie);

                if (wish != null)
                {
                    if (wish.HasUserId(userId))
                    {
                        wish.IdUsers.Remove(Guid.Parse(userId));

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

        #region Method Timer

        //private void TimerUpdate(object state)
        //{
        //    TorrentFinder torrentFinder = new TorrentFinder();
        //}

        #endregion
    }
}
