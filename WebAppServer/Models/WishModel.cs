using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class WishModel : MovieModel
    {
        #region Properties

        /// <summary>
        /// C'est la list des ID des utilisateurs qui ont ajoutés ce film.
        /// </summary>
        public List<Guid> IdUsers { get; set; }

        #endregion
        
        #region Constructeur

        public WishModel(Guid idMovie, List<Guid> idUsers) 
            : base(idMovie)
        {
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

        #endregion
    }
}
