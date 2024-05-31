using System.Net.Http.Json;
using System.Web;
using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Modules.OpenMeteoService
{
    public sealed class OpenMeteoService : IOpenMeteoService
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
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["latitude"] = latitude.ToString();
            query["longitude"] = longitude.ToString();
            query["start_date"] = startDate;
            query["end_date"] = endDate;
            query["hourly"] = "temperature_2m,relative_humidity_2m,surface_pressure";

            var requestUri = $"{_archiveApiUrl}?{query.ToString()}";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<WeatherData>();
        }
    }
}
