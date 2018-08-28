using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using WepAppServer.Data;

namespace WebAppServer.Pages.Setting
{
    public class UsersManagerModel : PageModel
    {
		#region Properties

		private readonly ApplicationDbContext _appContext;

		private const string MEMBER = "MEMBER";
		private const string MANAGER = "MANAGER";



		[BindProperty]
		public IEnumerable<ApplicationUser> AllUsers { get; set; }

		#endregion


		#region Constructeur

		public UsersManagerModel(ApplicationDbContext context)
		{
			_appContext = context;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Retourne la liste des Managers et des membres.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ApplicationUser> GetAllUser()
		{
			List<ApplicationUser> usersList = new List<ApplicationUser>();

			try
			{
				// Récupération des ids pour les rôles de membre et manager.
				IEnumerable<string> idsRoles = _appContext.Roles.Where(x => x.NormalizedName == MANAGER
																			|| x.NormalizedName == MEMBER)
					.Select(x => x.Id)
					.ToList();

				// Récupération des utilisateurs ayant pour ces rôles.
				IEnumerable<string> idUserRole = _appContext.UserRoles.Where(x => idsRoles.Contains(x.RoleId))
					.Select(x => x.UserId)
					.ToList();

				// Récupération des utilisateurs.
				usersList = _appContext.Users.Where(x => idUserRole.Contains(x.Id)).ToList();
			}
			catch (Exception exception)
			{
				Log.Error(exception, "UserManager.cshtml - Erreur dans la récupération de la liste des utilisateurs Membre et Manager.");
			}

			return usersList;
		}

		#endregion


        public void OnGet()
		{
			IEnumerable<ApplicationUser> allUsers = GetAllUser();
		}
    }
}