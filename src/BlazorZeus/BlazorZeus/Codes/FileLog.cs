namespace BlazorZeus.Codes
{
	public class FileLog
	{
		public string NameFile { get; set; }

		public string PathFile { get; set; }

		public FileLog(string nameFile, string pathFile)
		{
			NameFile = nameFile;
			PathFile = pathFile;
		}
	}
}
