using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorZeus.ViewModel
{
	public interface IUsersViewModel
	{
		List<UserView> AllUsers { get; set; }
		NavigationManager Navigation { get; set; }
		UserManager<IdentityUser> UserManager { get; set; }
		bool ShowResetMdp { get; set; }

		void OnChangeRole(ChangeEventArgs e, string idUser);

		void DeleteUser(string idUser);

		void OpenChangeMdp(string idUser);

		void CancelChangeMdp();

		Task SetNewPassword(string newPassword);
	}
}