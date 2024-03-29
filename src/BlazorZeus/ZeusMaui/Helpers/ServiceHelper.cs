﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusMaui.Helpers
{
	public static class ServiceHelper
	{
		public static IServiceProvider Current =>
#if WINDOWS10_0_17763_0_OR_GREATER
			MauiWinUIApplication.Current.Services;
#elif ANDROID
			MauiApplication.Current.Services;
#else
			null;
#endif



		public static TSvc GetService<TSvc>() => Current.GetService<TSvc>();

	}
}
