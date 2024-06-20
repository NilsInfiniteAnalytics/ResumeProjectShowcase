using ClassLibrary.Interfaces;
using ClassLibrary.Models.OpenMeteoService;
using ClassLibrary.Modules.OpenMeteoService;
namespace ClassLibrary.Tests.Modules;

[TestClass]
public sealed class OpenMeteoServiceTests
{
    private IOpenMeteoService _openMeteoService;
    private HttpClient _httpClient;

    [TestInitialize]
    public Task TestInitialize()
    {
        _httpClient = new HttpClient();
        _openMeteoService = new OpenMeteoService(_httpClient, "https://archive-api.open-meteo.com/v1/archive");
        return Task.CompletedTask;
    }

    [TestMethod]
    public async Task GetWeatherDataAsync_ReturnsWeatherData()
    {
        // Arrange
        const double latitude = 43.258509;
        const double longitude = -77.606445;
        var latLng = new LatLng
        {
            Latitude = latitude,
            Longitude = longitude
        };
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));

        // Act
        var result = await _openMeteoService.GetWeatherDataAsync(latLng, startDate, endDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.HourlyWeatherDataLists.Temperature2m.Count > 0);
        Assert.IsTrue(result.HourlyWeatherDataLists.RelativeHumidity2m.Count > 0);
        Assert.IsTrue(result.HourlyWeatherDataLists.SurfacePressure.Count > 0);
    }

    [TestMethod]
    public async Task GetWeatherDataAsync_ReturnsConvertedWeatherData()
    {
        // Arrange
        const double latitude = 43.258509;
        const double longitude = -77.606445;
        var latLng = new LatLng
        {
            Latitude = latitude,
            Longitude = longitude
        };
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));

        // Act
        var result = await _openMeteoService.GetWeatherDataAsync(latLng, startDate, endDate);
        Assert.IsNotNull(result);
        var temperatureArray = result.HourlyWeatherDataLists.Temperature2m.ToArray();
        var convertedTemperatureArray = UnitsConverter.Convert.ArrayFrom(temperatureArray, "\u00b0C").ToSIMD("\u00b0F");

        // Assert
        Assert.IsNotNull(convertedTemperatureArray);
    }
}
