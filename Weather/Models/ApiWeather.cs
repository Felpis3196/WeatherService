using System.Web;

namespace Weather.Models
{
    public class ApiWeather
    {
        private readonly HttpClient _httpClient;
        public ApiWeather(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetDataFromWeatherApiAsync(string lat, string lon)
        {
            var apiKey = "f8c57887056bfdcf8836da230f68aebf";
            var baseUrl = "https://api.openweathermap.org/data/2.5/weather";

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["lat"] = lat;
            query["lon"] = lon;
            query["appid"] = apiKey;
            query["units"] = "metric";
            query["lang"] = "pt_br";   

            uriBuilder.Query = query.ToString();

            var response = await _httpClient.GetAsync(uriBuilder.ToString());

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Erro ao acessar a WeatherAPI: {response.StatusCode}");
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException($"Erro ao acessar a API: {response.StatusCode}");
        }
    }
}
