using ACS.API.Services;
using DAL.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACS.API.Controllers
{
	[Route("/[controller]")]
	[ApiController]
	public class SUController : ControllerBase
	{
		private readonly IShortenerURLService _service;
		protected readonly ILogger _logger;
		protected readonly IConfiguration _configuration;

		public SUController(IShortenerURLService service, IConfiguration configuration, ILogger<SUController> logger)
		{
			this._service = service;
			this._logger = logger;
			this._configuration = configuration;

		}

		[HttpGet]
		public async Task<IActionResult> Get(string u)
		{

			ShortenerURL response = new ShortenerURL();
			response = _service.FindShortUrlByCodition(u);
			string longURL = "";
			if (response != null)
			{
				longURL = response.LongUrl;
			}
			if (string.IsNullOrEmpty(longURL))
			{
				return Ok(longURL);
			}
			else
			{
				return Redirect(longURL);
			}
		}
	}
}
