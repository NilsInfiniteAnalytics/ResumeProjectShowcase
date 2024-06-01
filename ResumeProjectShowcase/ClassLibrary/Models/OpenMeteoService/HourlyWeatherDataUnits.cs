namespace ClassLibrary.Models.OpenMeteoService
{
    public record HourlyWeatherDataUnits
    {
        public string Time { get; set; } = null!;
        public string Temperature2m { get; set; } = null!;
        public string RelativeHumidity2m { get; set; } = null!;
        public string SurfacePressure { get; set; } = null!;
    }
}
