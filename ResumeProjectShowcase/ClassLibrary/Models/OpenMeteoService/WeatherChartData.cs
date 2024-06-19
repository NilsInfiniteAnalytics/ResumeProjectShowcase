namespace ClassLibrary.Models.OpenMeteoService
{
    public record WeatherChartData
    {
        public string? Time { get; set; }
        public double Value { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
