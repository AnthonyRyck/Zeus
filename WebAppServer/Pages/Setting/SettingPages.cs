using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppServer.Pages.Setting
{
    public static class SettingPages
    {
	    public static string Configuration => "Configuration";

	    public static string UsersManager => "UsersManager";

	    public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Configuration);

	    public static string UserManageNavClass(ViewContext viewContext) => PageNavClass(viewContext, UsersManager);

	    public static string PageNavClass(ViewContext viewContext, string page)
	    {
		    var activePage = viewContext.ViewData["ActivePage"] as string
		                     ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
		    return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
	    }
	}
}
