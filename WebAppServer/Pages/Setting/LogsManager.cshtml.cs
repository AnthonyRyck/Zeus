using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppServer.Models;

namespace WebAppServer.Pages.Setting
{
    public class LogsManagerModel : PageModel
    {
	    public SelectList ListLogs { get; set; }

	    private IHostingEnvironment _environment;


	    private List<LogFile> _allLogFiles = new List<LogFile>();

	    [BindProperty]
		public string FileLogSelected { get; set; }

	    public IEnumerable<string> Logs { get; set; } = new List<string>();

	    public LogsManagerModel(IHostingEnvironment environment)
	    {
		    _environment = environment;
	    }


        public void OnGet()
        {
	        string[] tempFiles = Directory.GetFiles(Path.Combine(_environment.ContentRootPath,"Logs"));

	        foreach (string filePath in tempFiles)
	        {
				// Pour éviter de prendre le log de "System".
				if (!filePath.Contains("system"))
				{
					_allLogFiles.Add(new LogFile(filePath));
				}
			}

			ListLogs = new SelectList(_allLogFiles, "FullPath", "DateEnLettre");
		}


	    public async Task<IActionResult> OnPostLoadAsync()
	    {
		    bool stop = true;

		    Logs = (await System.IO.File.ReadAllLinesAsync(FileLogSelected)).Reverse();
			
		    return Page(); //RedirectToPage();
	    }

	}
}