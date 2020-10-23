using BlazorZeus.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Pages
{
	public partial class GestionUserPage
	{
		#region Properties

		[Inject]
		private ApplicationDbContext AppContext { get; set; }

		[Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public UserManager<IdentityUser> UserManager { get; set; }

		private const string MANAGER = "MANAGER";
		private const string MEMBER = "MEMBER";

		public List<UserView> AllUsers { get; set; }

		#endregion

		#region Constructeur

		public GestionUserPage()
		{
			AllUsers = new List<UserView>();
		}

		protected override void OnInitialized()
		{
			AllUsers = GetAllUser().ToList();
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Retourne la liste des Managers et des membres.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<UserView> GetAllUser()
		{
			List<UserView> usersList = new List<UserView>();

			try
			{
				// Récupération des ids pour les rôles de membre et manager.
				IEnumerable<string> idsRoles = AppContext.Roles.Where(x => x.NormalizedName == MANAGER
																		|| x.NormalizedName == MEMBER)
																.Select(x => x.Id)
																.ToList();

				// Récupération des utilisateurs ayant pour ces rôles.
				IEnumerable<string> idUserRole = AppContext.UserRoles.Where(x => idsRoles.Contains(x.RoleId))
					.Select(x => x.UserId)
					.ToList();

				// Récupération des utilisateurs.
				IEnumerable<IdentityUser> usersTemp = AppContext.Users.Where(x => idUserRole.Contains(x.Id)).ToList();

				foreach (var user in usersTemp)
				{
					string roleId = AppContext.UserRoles.Where(x => x.UserId == user.Id)
											.Select(x => x.RoleId)
											.FirstOrDefault();

					string role = AppContext.Roles.Where(x => x.Id == roleId)
													.Select(x => x.NormalizedName)
													.FirstOrDefault();

					UserView userView = new UserView();
					userView.User = user;
					userView.Role = role;

					usersList.Add(userView);
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, "GestionUserPage - Erreur dans la récupération de la liste des utilisateurs Membre et Manager.");
			}

			return usersList;
		}

		/// <summary>
		/// Au changement de rôle.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idUser"></param>
		private void OnChangeRole(ChangeEventArgs e, string idUser)
		{
			var selectedValue = e.Value.ToString();
			UserView currentUser = AllUsers.Where(x => x.User.Id == idUser).FirstOrDefault();

			if (string.IsNullOrEmpty(selectedValue))
				return;

			if(selectedValue == "Inactif")
			{
				UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
			}
			else
			{
				UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
				UserManager.AddToRoleAsync(currentUser.User, selectedValue);
			}

			AllUsers = GetAllUser().ToList();
			StateHasChanged();
		}

		private void DeleteUser(string idUser)
		{
			if (AppContext.Users.Any(x => x.Id == idUser))
			{
				var user = AppContext.Users.FirstOrDefault(x => x.Id == idUser);

				AppContext.Users.Remove(user);
				AppContext.SaveChanges();

				AllUsers.RemoveAll(x => x.User.Id == idUser);
			}
		}
		#endregion





	}

	public class UserView
	{
		public IdentityUser User { get; set; }

		public string Role { get; set; }
	}

}
