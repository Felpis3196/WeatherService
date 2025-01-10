using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Weather.Models;

namespace Weather.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherService _weatherService;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        public IActionResult Index(string Cidade)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string cityName, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                ModelState.AddModelError("cityName", "O nome da cidade é obrigatório.");
                return View();
            }

            try
            {
                var weatherData = await _weatherService.GetWeatherByLocationAsync(cityName, countryCode);
                ViewData["WeatherData"] = 1;
                return View(weatherData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao obter dados do tempo: {ex.Message}");
            }

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
