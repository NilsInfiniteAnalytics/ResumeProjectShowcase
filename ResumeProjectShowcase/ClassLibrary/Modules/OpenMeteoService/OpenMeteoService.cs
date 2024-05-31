using System.Net.Http.Json;
using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Modules.OpenMeteoService
{
    public class OpenMeteoService : IOpenMeteoService
    {
        private readonly HttpClient _httpClient;

        private readonly string _archiveApiUrl;

        public OpenMeteoService(HttpClient httpClient, string archiveApiUrl)
        {
            _httpClient = httpClient;
            _archiveApiUrl = archiveApiUrl;
        }

        public async Task<WeatherData?> GetWeatherDataAsync(double latitude, double longitude, string startDate, string endDate)
        {
            var parameters = new
            {
                latitude,
                longitude,
                start_date = startDate,
                end_date = endDate,
                hourly = new[] { "temperature_2m", "relative_humidity_2m", "surface_pressure" },
            };
            var response = await _httpClient.PostAsJsonAsync(_archiveApiUrl, parameters);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<WeatherData>();
        }
    }
}
