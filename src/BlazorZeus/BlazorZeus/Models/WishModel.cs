using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Models
{
    public class WishModel
    {
        #region Properties

        /// <summary>
        /// C'est la list des ID des utilisateurs qui ont ajoutés ce film.
        /// </summary>
        public List<Guid> IdUsers { get; set; }


        /// <summary>
        /// C'est le film qui est dans la liste de souhait.
        /// </summary>
        public MovieWishModel Movie { get; set; }

        #endregion
        
        #region Constructeur

        public WishModel(MovieWishModel movie, List<Guid> idUsers)
        {
            Movie = movie;
            IdUsers = idUsers;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indique si l'ID passé en paramètre est dans la liste IdUsers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasUserId(string userId)
        {
            return IdUsers.Any(x => x.ToString() == userId);
        }

        /// <summary>
        /// Indique si l'ID passé en paramètre est dans la liste IdUsers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasUserId(Guid userId)
        {
            return IdUsers.Any(x => x == userId);
        }

        #endregion
    }
}
