using static Weather.Models.ApiWeather;
using System.Text.Json;

namespace Weather.Models
{
    public class WeatherService
    {
        private readonly ApiWeather _apiWeather;
        private readonly ApiGeo _apiGeo;

        public WeatherService(ApiWeather weatherApi, ApiGeo geoApi)
        {
            _apiWeather = weatherApi;
            _apiGeo = geoApi;
        }

        public async Task<WeatherResponse> GetWeatherByLocationAsync(string cityName, string? countryCode)
        {
            var (lat, lon) = await _apiGeo.GetCoordinatesAsync(cityName, countryCode);
            var weatherData = await _apiWeather.GetDataFromWeatherApiAsync(lat, lon);
            return weatherData;
        }

    }

}
