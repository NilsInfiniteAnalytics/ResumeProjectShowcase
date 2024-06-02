namespace ClassLibrary.Models.OpenMeteoService
{
    public record HourlyWeatherDataLists
    {
        public List<string> Time { get; set; } = [];
        public List<double> Temperature2m { get; set; } = [];
        public List<double> RelativeHumidity2m { get; set; } = [];
        public List<double> SurfacePressure { get; set; } = [];
    }
}
