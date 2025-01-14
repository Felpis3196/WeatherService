using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Weather.Controllers;

namespace Weather.Models
{
    public class ApiGeo
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;
        public ApiGeo(HttpClient httpClient, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["WeatherApi:ApiKey"]
                      ?? throw new InvalidOperationException("A chave da API não está configurada.");
        }

        public async Task<(double lat, double lon)> GetCoordinatesAsync(string cityName, string countryCode = "", int limit = 1)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("O nome da cidade é obrigatório.", nameof(cityName));

            var baseUrl = "http://api.openweathermap.org/geo/1.0/direct";

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["q"] = $"{cityName.Trim()},{countryCode.Trim()}".Trim(',');
            query["limit"] = limit.ToString();
            query["appid"] = _apiKey;

            uriBuilder.Query = query.ToString();

            var response = await _httpClient.GetAsync(uriBuilder.ToString());

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro ao acessar a GeoAPI: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"GeoAPI Response JSON: {json}"); 

            var geoData = JsonSerializer.Deserialize<GeoResponse[]>(json);

            if (geoData == null || geoData.Length == 0)
                throw new Exception($"Nenhuma coordenada encontrada para a cidade: {cityName}");

            var firstResult = geoData[0];
            return (firstResult.Lat, firstResult.Lon);
        }
        private class GeoResponse
        {
            public string Name { get; set; }
            public Dictionary<string, string> LocalNames { get; set; }
            [JsonPropertyName("lat")]
            public double Lat { get; set; }
            [JsonPropertyName("lon")]
            public double Lon { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
        }

    }
}
