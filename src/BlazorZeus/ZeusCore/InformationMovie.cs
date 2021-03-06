using System;
using System.Collections.Generic;
using System.Text;

namespace ZeusCore
{
	public class InformationMovie
	{
		public Guid IdMovie { get; set; }
		
		public string Titre { get; set; }
		
		public DateTime DateAdded { get; set; }
		
		public string PosterPath { get; set; }
				
		public string ImgSource => ("https://image.tmdb.org/t/p/w370_and_h556_bestv2" + PosterPath);
	}
}
