using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Interfaces
{
    public interface IOpenMeteoService
    {
        public Task<WeatherData?> GetWeatherDataAsync(LatLng latLng, DateOnly startDate, DateOnly endDate);
    }
}
