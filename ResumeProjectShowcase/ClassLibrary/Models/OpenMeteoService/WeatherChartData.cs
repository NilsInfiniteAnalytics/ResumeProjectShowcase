namespace ClassLibrary.Models.OpenMeteoService
{
    public record WeatherChartData
    {
        public string? Time { get; set; }
        public double Value { get; set; }
    }
}
