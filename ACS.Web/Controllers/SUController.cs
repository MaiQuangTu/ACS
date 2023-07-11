using ACS.Web.Common;
using ACS.Web.Services;
using DAL.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace ACS.Web.Controllers
{
	public class SUController : BaseController
	{
		private readonly IShortenerURLService _service;
		private readonly ILogger<SUController> _logger;

		public SUController(IShortenerURLService service, IWebHostEnvironment environment, ILogger<SUController> logger) : base(environment, logger)
		{
			_service = service;
			_logger = logger;
		}

		public IActionResult Index(string? u)
		{
			ShortenerURL response = new ShortenerURL();
			response = _service.FindShortUrlByCodition(u);
			return Redirect(response.LongUrl);
		}

		[HttpGet]
		public  IActionResult GetShortURL(string u)
		{
			ShortenerURL response = new ShortenerURL();
			response = _service.FindShortUrlByCodition(u);
			return Ok(response);
		}

		[HttpGet]
		public IActionResult GetLongURL(string u)
		{
			ShortenerURL response = new ShortenerURL();
			response = _service.FindLongUrlByCodition(u);
			return Ok(response);
		}

		[HttpPut]
		public async Task<IActionResult> CreateShortUrl(string u)
		{
			try
			{
				ShortenerURL model = await _service.Add(u);
				string shortUrl = Constant.RedirecLink + model.ShortUrl;
				return Ok(new { ShortURl = shortUrl });
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}

		}
	}
}