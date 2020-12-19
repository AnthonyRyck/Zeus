using BlazorZeus.Codes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Composants.ViewModel
{
	public interface IGestionLog
	{

		public List<FileLog> FilesLog { get; set; }

		public string TextLogSelected { get; set; }

		public string[] AllTextLogSelected { get; set; }



		public void SelectFile(string pathFile);
	}
}
