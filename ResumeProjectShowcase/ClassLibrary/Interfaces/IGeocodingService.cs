using ClassLibrary.Models.OpenMeteoService;

namespace ClassLibrary.Interfaces
{
    public interface IGeocodingService
    {
        public Task<string> GetCityNameAsync(LatLng latLng);
    }
}
