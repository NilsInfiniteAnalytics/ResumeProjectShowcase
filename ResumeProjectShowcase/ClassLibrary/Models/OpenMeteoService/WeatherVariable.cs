namespace ClassLibrary.Models.OpenMeteoService
{
    public class WeatherVariable
    {
        public string Name { get; set; } = null!;
        public List<double> Values { get; set; } = null!;
    }
}
