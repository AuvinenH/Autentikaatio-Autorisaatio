using AutentikaatioAutorisaatio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly TokenService _tokenService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
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
                var token = _tokenService.GenerateToken(credentials.Username, false);
                return Ok(new { Token = token });
            }

            if (credentials.Username == "admin" && credentials.Password == "adminpassword")
            {
                var token = _tokenService.GenerateToken(credentials.Username, true);
                return Ok(new { Token = token });
            }

            return Unauthorized("Käyttäjätunnus tai salasana on väärin.");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetSecret")]
        public IActionResult GetSecretData()
        {
            return Ok("Tama on suojattua tietoa vain Admin-roolia käyttaviltä.");
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

        public class UserCredentials
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
