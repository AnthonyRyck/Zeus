using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Composants.ViewModel
{
	public class GestionLog : IGestionLog
	{
		#region Properties

		public List<FileInfo> FilesLogsCollection { get; set; }

		public List<string> TextLog { get; set; }
		public string NomDuFichier { get; set; }

		private string _localPath;

		#endregion

		#region Constructeur

		public GestionLog()
		{
			_localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

			FilesLogsCollection = GetListFiles(); //.GetAwaiter().GetResult();
		}

		#endregion

		#region Public Methods

		public List<FileInfo> GetListFiles()
		{
			List<FileInfo> filesCollection = new List<FileInfo>();

			try
			{
				//filesCollection = await Task.Factory.StartNew(() => 
				//{
					List<FileInfo> files = new List<FileInfo>();

					var tempFiles = Directory.EnumerateFiles(_localPath).ToList();

					foreach (var file in tempFiles)
					{
						FileInfo fileInfo = new FileInfo(file);
					//files.Add(fileInfo);

					filesCollection.Add(fileInfo);
					}

				//	return files;
				//});

			}
			catch (Exception ex)
			{
				Log.Logger.Error("Erreur sur la récupération de la liste des fichiers de Logs", ex);
			}

			return filesCollection.OrderBy(x => x.CreationTime).ToList();
		}

		public async Task SelectFile(string fileName)
		{
			var fileSelected = FilesLogsCollection.Where(x => x.Name == fileName).FirstOrDefault();

			NomDuFichier = fileSelected.Name;
			TextLog = (await File.ReadAllLinesAsync(fileSelected.FullName)).ToList();
		}

		#endregion


	}
}
