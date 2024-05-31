namespace ClassLibrary.Models.OpenMeteoService
{
    public class WeatherData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = null!;
        public string TimezoneAbbreviation { get; set; } = null!;
        public int UtcOffsetSeconds { get; set; }
        public HourlyData HourlyData { get; set; } = null!;
    }
}
