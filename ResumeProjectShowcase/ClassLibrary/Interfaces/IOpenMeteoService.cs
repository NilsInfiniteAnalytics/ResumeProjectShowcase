using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Interfaces
{
    public interface IOpenMeteoService
    {
        public Task<WeatherData?> GetWeatherDataAsync(double latitude, double longitude, string startDate, string endDate);
    }
}
