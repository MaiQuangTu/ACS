using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ACS.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class GenerateTokenController : ControllerBase
	{
		protected readonly IConfiguration _configuration;

		public GenerateTokenController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet("Generate")]
		public async Task<IActionResult> GenerateToken(string u)
		{
			try
			{
				var claims = new[] {
						new Claim("UserId", "ACS"),
						new Claim("EmpNo", u)
					};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
				var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					_configuration["Jwt:Issuer"],
					_configuration["Jwt:Audience"],
					claims,
					expires: DateTime.UtcNow.AddMinutes(15),
					signingCredentials: signIn);
				var access_token = new JwtSecurityTokenHandler().WriteToken(token);
				return Ok(access_token);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
