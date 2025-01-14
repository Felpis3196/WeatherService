using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Weather.Models
{
    public class ApiWeather
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        public ApiWeather(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApi:ApiKey"]
                      ?? throw new InvalidOperationException("A chave da API não está configurada.");
        }
        public class WeatherResponse
        {
            public MainInfo Main { get; set; }
            public Weather[] Weather { get; set; }
            public Wind Wind { get; set; }
            public string Name { get; set; }
        }
        public class MainInfo
        {
            public double Temp { get; set; }
            public int Humidity { get; set; }
            [JsonPropertyName("feels_like")]
            public double FeelsLike { get; set; }

            [JsonPropertyName("temp_max")]
            public double TempMax { get; set; }

            [JsonPropertyName("temp_min")]
            public double TempMin { get; set; }
        }

        public class Weather
        {
            public string Description { get; set; }
        }
        public class Wind
        {
            [JsonPropertyName("visibility")]
            public double Visibilidade { get; set; } 
            [JsonPropertyName("speed")]
            public double Velocidade { get; set; }
        }
                                                        
        public async Task<WeatherResponse> GetDataFromWeatherApiAsync(double lat, double lon)
        {
            var baseUrl = "https://api.openweathermap.org/data/2.5/weather";

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["lat"] = lat.ToString();
            query["lon"] = lon.ToString();
            query["appid"] = _apiKey;
            query["units"] = "metric";
            query["lang"] = "pt_br";

            uriBuilder.Query = query.ToString();

            var response = await _httpClient.GetAsync(uriBuilder.ToString());

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Erro ao acessar a WeatherAPI: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherData == null)
            {
                throw new Exception("Erro ao desserializar a resposta da API.");
            }

            return weatherData;
        }
    }
}
