﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZeusMobile.Models;

namespace ZeusMobile.Services
{
	public class SettingManager
	{
		public Setting Setting { get; set; }

		public string PathSetting{ get; set; }

		public SettingManager()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			PathSetting = Path.Combine(path, "settings.info");
		}

		internal void LoadSetting()
		{
			if (File.Exists(PathSetting))
			{
				var json = File.ReadAllText(PathSetting);
				Setting = JsonConvert.DeserializeObject<Setting>(json);
			}
			else
			{
				Setting = new Setting();
				SaveFile();
			}

			return;
		}


		public void SaveServeur(string adresseSvr)
		{
			try
			{
				Setting.AddressServer = adresseSvr;
				SaveFile();
			}
			catch (Exception ex)
			{
				var dd = ex;
				throw;
			}
		}

		private IZeusService ZeusService => DependencyService.Get<IZeusService>();

		private void SaveFile()
		{
			string content = JsonConvert.SerializeObject(Setting);
			File.WriteAllText(PathSetting, content);

			if(ZeusService != null)
				ZeusService.ChangeServerAddress(Setting.AddressServer);
		}
	}
}
