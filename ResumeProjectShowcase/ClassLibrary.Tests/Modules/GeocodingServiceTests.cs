using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;
using ClassLibrary.Modules.GeocodingService;

namespace ClassLibrary.Tests.Modules;

[TestClass]
public sealed class GeocodingServiceTests
{
    private IGeocodingService _geocodingService;
    private HttpClient _httpClient;

    [TestInitialize]
    public Task TestInitialize()
    {
        _httpClient = new HttpClient();
        _geocodingService = new GeocodingService(_httpClient);
        return Task.CompletedTask;
    }

    [TestMethod]
    public async Task GetWeatherDataAsync_ReturnsWeatherData()
    {
        // Arrange
        const double latitude = 43.1566;
        const double longitude = -77.6088;
        var latLng = new LatLng
        {
            Latitude = latitude,
            Longitude = longitude
        };

        // Act
        var result = await _geocodingService.GetCityNameAsync(latLng);

        // Assert
        Assert.IsNotNull(result);
    }
}
