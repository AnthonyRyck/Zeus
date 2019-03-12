using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using WebAppServer.Codes.Wish;

namespace WebAppServer.Pages.Wish
{
    [Authorize]
    public class WishListModel : PageModel
    {
        public async void OnGet()
        {
            Log.Debug("Consultation page - WishList -");
            Wishes = await _wishMaster.GetWishes();
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