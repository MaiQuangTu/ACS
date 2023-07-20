using ACS.API.Common;
using ACS.API.Services;
using DAL.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ShortenerURLController : ControllerBase
	{
		private readonly IShortenerURLService _service;
		protected readonly ILogger _logger;

		public ShortenerURLController(IShortenerURLService service, ILogger<ShortenerURLController> logger)
		{
			_service = service;
			this._logger = logger;
		}

		[HttpGet("GetShortUrl")]
		public async Task<IActionResult> GetShortURL(string u)
		{
			ShortenerURL response = new ShortenerURL();
			string shortUrl = "";
			try
			{
				response = _service.FindLongUrlByCodition(u);
				if (response != null)
				{
					shortUrl = Constant.RedirecLink + response.ShortUrl;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return Ok(shortUrl);
		}

		[HttpGet("GetLongUrl")]
		public async Task<IActionResult> GetLongURL(string u)
		{
			ShortenerURL response = new ShortenerURL();
			response = _service.FindShortUrlByCodition(u);
			string longUrl = "";
			if (response != null)
			{
				longUrl = response.LongUrl;
			}
			return Ok(longUrl);
		}

		[HttpPut("CreateShortUrl")]
		public async Task<IActionResult> CreateShortUrl(string u)
		{
			try
			{
				string shortUrl = "";
				ShortenerURL longUrlRes = new ShortenerURL();
				longUrlRes = _service.FindLongUrlByCodition(u);
				if (longUrlRes != null)
				{
					shortUrl = Constant.RedirecLink + longUrlRes.ShortUrl;
					return Ok(shortUrl);
				}
				else
				{
					ShortenerURL model = await _service.Add(u);

					shortUrl = Constant.RedirecLink + model.ShortUrl;
					return Ok(shortUrl);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return BadRequest("Error: Create Short Url failed");
			}
		}
	}
}
