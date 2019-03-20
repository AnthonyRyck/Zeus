using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using WebAppServer.Codes.Wish;
using WebAppServer.Extensions;

namespace WebAppServer.Pages.Wish
{
    [Authorize]
    public class WishListModel : PageModel
    {
        public void OnGet()
        {
            Log.Debug("Consultation page - WishList -");
            
            if (this.User.IsInRole("Manager") || this.User.IsInRole("Admin"))
            {
                Wishes = _wishMaster.GetWishes();
            }
            else
            {
                string userId = this.User.GetUserId();
                Wishes = _wishMaster.GetWishes(userId);
            }
            
            Log.Debug("Page WishList - Wish = " + Wishes.Count() + " films.");
        }

        #region Fields

        private readonly IWish _wishMaster;

        #endregion

        #region Properties

        /// <summary>
        /// Liste des souhaits en film.
        /// </summary>
        public IEnumerable<Models.WishModel> Wishes { get; private set; }

        #endregion

        #region Constructeur

        public WishListModel(IWish wishManager)
        {
            _wishMaster = wishManager;
        }

        #endregion
    }
}