namespace ClassLibrary.Models.OpenMeteoService
{
    public record HourlyWeatherDataLists
    {
        public List<string> Time { get; set; } = null!;
        public List<double> Temperature2m { get; set; } = null!;
        public List<double> RelativeHumidity2m { get; set; } = null!;
        public List<double> SurfacePressure { get; set; } = null!;
    }
}
