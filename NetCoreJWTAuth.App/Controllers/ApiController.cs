using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NetCoreJWTAuth.App.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IConfiguration _configuration;

        public ApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("Super secret content, I hope you've got clearance for this...");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (request.Username == "Jon" && request.Password == "Again, not for production use, DEMO ONLY!")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }

    }

    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}