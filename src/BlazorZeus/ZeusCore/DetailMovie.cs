using System;
using System.Collections.Generic;
using System.Text;

namespace ZeusCore
{
	public class DetailMovie
	{
		public Guid IdMovie { get; set; }
		public string Titre { get; set; }
		public string Annee { get; set; }
		public string Resolution { get; set; }
		public string Qualite { get; set; }
		public string FileName { get; set; }
		public long Size { get; set; }

		public DateTime DateAdded { get; set; }
		public string PosterPath { get; set; }

		public string ImgSource => ("https://image.tmdb.org/t/p/w370_and_h556_bestv2" + PosterPath);

		public string Overview { get; set; }

		public List<Video> Videos { get; set; }

	}
}
