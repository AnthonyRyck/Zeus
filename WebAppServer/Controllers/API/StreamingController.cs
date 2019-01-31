using System;
using Microsoft.AspNetCore.Mvc;
using WebAppServer.Codes;

namespace WebAppServer.Controllers.API
{
	[Route("api/[controller]")]
	public class StreamingController : Controller
    {
		private IVideoStreamService _streamingService;

		public StreamingController(IVideoStreamService streamingService)
		{
			_streamingService = streamingService;
		}

		[HttpGet("{id}")]
		public IActionResult GetStream(Guid id)
		{
			return _streamingService.GetVideoById(id);
		}
	}
}