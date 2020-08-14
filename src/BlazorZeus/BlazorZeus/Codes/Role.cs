using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Codes
{
	public enum Role
	{
		Admin,
		Manager,
		Member
	}


	public static class RoleExtension
	{
		public static Role ToRole(this string roleString)
		{
			Role role = Role.Member;

			switch (roleString)
			{
				case "Admin":
					role = Role.Admin;
					break;

				case "Manager":
					role = Role.Manager;
					break;

				case "Member":
					role = Role.Member;
					break;
			}

			return role;
		}
	}
}
