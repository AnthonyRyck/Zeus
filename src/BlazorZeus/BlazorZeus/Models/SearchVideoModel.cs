using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorZeus.Models
{
    public class SearchVideoModel
    {
        public string UrlAffiche { get; set; }
        public int IdVideoTmDb { get; set; }
	    public string Titre { get; set; }
		public DateTime? ReleaseDate { get; internal set; }
	}
}
