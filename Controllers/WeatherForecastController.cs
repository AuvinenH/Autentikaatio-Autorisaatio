using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutentikaatioAutorisaatio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("OpenGet")]
        public IEnumerable<WeatherForecast> GetOpen()
        {
            return CreateForecasts();
        }

        [Authorize]
        [HttpGet("AuthGet")]
        public IEnumerable<WeatherForecast> GetProtected()
        {
            return CreateForecasts();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            if (credentials.Username == "testuser" && credentials.Password == "testpassword")
            {
                var token = GenerateToken(credentials.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized("Kayttajatunnus tai salasana on vaarin.");
        }

        private static IEnumerable<WeatherForecast> CreateForecasts()
        {
            return Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }

        private static string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_that_is_at_least_32_bytes_long"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: "MyTestAuthServer",
                audience: "MyTestApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class UserCredentials
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
