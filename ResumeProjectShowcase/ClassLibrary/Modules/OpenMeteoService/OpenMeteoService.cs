using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Web;
using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;
using Serilog;

namespace ClassLibrary.Modules.OpenMeteoService;
public sealed class OpenMeteoService : IOpenMeteoService
{
    private readonly HttpClient _httpClient;
    private readonly string _archiveApiUrl;
    private readonly ILogger _logger;

    public OpenMeteoService(HttpClient httpClient, string archiveApiUrl, ILogger logger)
    {
        _httpClient = httpClient;
        _archiveApiUrl = archiveApiUrl;
        _logger = logger;
    }

    public async Task<WeatherData?> GetWeatherDataAsync(LatLng latLng, DateOnly startDate, DateOnly endDate)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["latitude"] = latLng.Latitude.ToString(CultureInfo.InvariantCulture);
        query["longitude"] = latLng.Longitude.ToString(CultureInfo.InvariantCulture);
        var startDateString = startDate.ToString("yyyy-MM-dd");
        var endDateString = endDate.ToString("yyyy-MM-dd");
        query["start_date"] = startDateString;
        query["end_date"] = endDateString;
        query["hourly"] = "temperature_2m,relative_humidity_2m,surface_pressure";

        var requestUri = $"{_archiveApiUrl}?{query.ToString()}";
        _logger.Information($"Requesting weather data from OpenMeteo API: {requestUri}");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var response = await _httpClient.GetAsync(requestUri);
        stopwatch.Stop();
        _logger.Information($"Request took {stopwatch.ElapsedMilliseconds} ms");
        _logger.Information($"Response status code: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var stringData = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(stringData);
        var root = document.RootElement;
        try
        {
            var weatherData = new WeatherData
            {
                Latitude = root.GetProperty("latitude").GetDouble(),
                Longitude = root.GetProperty("longitude").GetDouble(),
                Timezone = root.GetProperty("timezone").GetString()!,
                TimezoneAbbreviation = root.GetProperty("timezone_abbreviation").GetString()!,
                UtcOffsetSeconds = root.GetProperty("utc_offset_seconds").GetInt32(),
                HourlyWeatherDataUnits = new HourlyWeatherDataUnits
                {
                    Time = root.GetProperty("hourly_units").GetProperty("time").GetString()!,
                    Temperature2m = root.GetProperty("hourly_units").GetProperty("temperature_2m").GetString()!,
                    RelativeHumidity2m = root.GetProperty("hourly_units").GetProperty("relative_humidity_2m").GetString()!,
                    SurfacePressure = root.GetProperty("hourly_units").GetProperty("surface_pressure").GetString()!
                },
                HourlyWeatherDataLists = new HourlyWeatherDataLists
                {
                    Time = root.GetProperty("hourly").GetProperty("time").EnumerateArray().Select(x => x.GetString()!).ToList(),
                    Temperature2m = root.GetProperty("hourly").GetProperty("temperature_2m").EnumerateArray().Select(x => x.GetDouble()).ToList(),
                    RelativeHumidity2m = root.GetProperty("hourly").GetProperty("relative_humidity_2m").EnumerateArray().Select(x => x.GetDouble()).ToList(),
                    SurfacePressure = root.GetProperty("hourly").GetProperty("surface_pressure").EnumerateArray().Select(x => x.GetDouble()).ToList()
                }
            };
            return weatherData;
        }
        catch (Exception e)
        {
            System.Diagnostics.Trace.TraceError($"Error: {e.Message}");
            return null;
        }
    }
}
