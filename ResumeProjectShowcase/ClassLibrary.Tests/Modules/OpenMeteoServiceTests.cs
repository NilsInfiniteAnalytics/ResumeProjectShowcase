using ClassLibrary.Interfaces;
using ClassLibrary.Modules.OpenMeteoService;
using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Tests.Modules
{
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
            const string startDate = "2024-05-15";
            const string endDate = "2024-05-29";

            // Act
            var result = await _openMeteoService.GetWeatherDataAsync(latitude, longitude, startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HourlyWeatherDataLists.Temperature2m.Count > 0);
            Assert.IsTrue(result.HourlyWeatherDataLists.RelativeHumidity2m.Count > 0);
            Assert.IsTrue(result.HourlyWeatherDataLists.SurfacePressure.Count > 0);
        }
    }
}
