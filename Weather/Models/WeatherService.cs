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

        public async Task<string> GetWeatherByLocationAsync(string cityName, string? countryCode)
        {
            var (lat, lon) = await _apiGeo.GetCoordinatesAsync(cityName, countryCode);
            return await _apiWeather.GetDataFromWeatherApiAsync(lat.ToString(), lon.ToString());
        }
    }

}
