using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;
using System.Net.Http.Json;
using ClassLibrary.Models.GeocodingService;

namespace ClassLibrary.Modules.GeocodingService
{
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        // This is a restricted key on my Google Maps Platform account. Replace it with your own key.
        private const string ApiKey = "AIzaSyDRRgoL4GaqyZkqpySaYXJVYKDXDbuGj9E";
        public GeocodingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetCityNameAsync(LatLng latLng)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latLng.Latitude},{latLng.Longitude}&key={ApiKey}";
            var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(url);

            return response?.Results?.FirstOrDefault()?.AddressComponents?
                .FirstOrDefault(ac => ac.Types.Contains("locality"))?.LongName ?? "Unknown";
        }
    }
}
