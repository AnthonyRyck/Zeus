﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlazorZeus.Codes
{
	public interface IVideoStreamService
	{
		FileStreamResult GetVideoById(Guid idVideo);
	}
}
