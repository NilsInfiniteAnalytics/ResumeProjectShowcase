using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;
using System.Net.Http.Json;
using ClassLibrary.Models.GeocodingService;
using Serilog;
using System.Diagnostics;

namespace ClassLibrary.Modules.GeocodingService;
public class GeocodingService(HttpClient httpClient, ILogger logger) : IGeocodingService
{
    // This is a restricted key on my Google Maps Platform account. Replace it with your own key.
    private const string ApiKey = "AIzaSyDRRgoL4GaqyZkqpySaYXJVYKDXDbuGj9E";
    /// <summary>
    /// <para>Status codes returned by the Geocoding API.</para>
    /// https://developers.google.com/maps/documentation/geocoding/requests-reverse-geocoding
    /// </summary>
    private enum ReverseGeocodingStatusCodes
    {
        OK,
        ZERO_RESULTS,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        INVALID_REQUEST,
        UNKNOWN_ERROR
    }

    public async Task<string> GetCityNameAsync(LatLng latLng)
    {
        var url =
            $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latLng.Latitude},{latLng.Longitude}&key={ApiKey}";
        logger.Information($"Requesting city name from Geocoding API: {url}");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            var response = await httpClient.GetAsync(url);
            stopwatch.Stop();
            logger.Information($"Request took {stopwatch.ElapsedMilliseconds} ms");
            logger.Information($"Response status code: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                return "Unknown";
            }
            var geocodingResponse = await response.Content.ReadFromJsonAsync<GeocodingResponse>();
            // Switch on the status code to handle different scenarios using the enum defined above.
            switch (geocodingResponse?.Status)
            {
                case nameof(ReverseGeocodingStatusCodes.OK):
                    var cityName = geocodingResponse?
                        .Results.FirstOrDefault()?
                        .AddressComponents.FirstOrDefault(ac => ac.Types.Contains("locality"))?.LongName ?? "Unknown";
                    return cityName;
                case nameof(ReverseGeocodingStatusCodes.ZERO_RESULTS):
                    logger.Error($"{geocodingResponse?.Status}");
                    return "Unknown";
                case nameof(ReverseGeocodingStatusCodes.OVER_QUERY_LIMIT):
                    logger.Error($"{geocodingResponse?.Status}");
                    return "Unknown";
                case nameof(ReverseGeocodingStatusCodes.REQUEST_DENIED):
                    logger.Error($"{geocodingResponse?.Status}");
                    return "Unknown";
                case nameof(ReverseGeocodingStatusCodes.INVALID_REQUEST):
                    logger.Error($"{geocodingResponse?.Status}");
                    return "Unknown";
                case nameof(ReverseGeocodingStatusCodes.UNKNOWN_ERROR):
                    logger.Error($"{geocodingResponse?.Status}");
                    return await GetCityNameAsync(latLng);
                default:
                    return "Unknown";
            }
        }
        catch (HttpRequestException e)
        {
            logger.Error(e, "Error while requesting city name from Geocoding API");
            return "Unknown";
        }
        
    }
}