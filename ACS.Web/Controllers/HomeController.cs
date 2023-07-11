using ACS.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ACS.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			Constant.ACCESS_TOKEN = GenerateToken();
			return View();
		}
		private static string GenerateToken()
		{

			// Set Time Expiry is 2050-12-31 23:59:59
			DateTime timeExpiry = new DateTime(2050, 12, 31, 23, 59, 59);
			TimeSpan sinceEpoch = timeExpiry - new DateTime(1970, 1, 1);
			long expiry = (long)sinceEpoch.TotalSeconds;
			string expiryStr = expiry.ToString();
			string stringToSign = HttpUtility.UrlEncode("ShortenerUrl") + "\n" + expiryStr;
			HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes("ACS"));
			string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
			string token = $"SharedAccessSignature sr={HttpUtility.UrlEncode("shortenerACS")}&sig={HttpUtility.UrlEncode(signature)}&se={expiry}&skn={"keyName"}";

			return token;
			//// Set Time Expiry is 2050-12-31 23:59:59
			//DateTime timeExpiry = new DateTime(2050, 12, 31, 23, 59, 59);
			//TimeSpan sinceEpoch = timeExpiry - new DateTime(1970, 1, 1);
			//long expiry = (long)sinceEpoch.TotalSeconds;
			//string expiryStr = expiry.ToString();

			//string token = $"SharedAccessSignature se={expiry}";

			//return token;
		}
	}
}