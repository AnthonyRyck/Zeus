using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Composants.ViewModel
{
	public interface IGestionLog
	{

		public List<FileInfo> FilesLogsCollection { get; set; }


		public string NomDuFichier { get; set; }
		public List<string> TextLog { get; set; }


		Task SelectFile(string fileName);
	}
}
