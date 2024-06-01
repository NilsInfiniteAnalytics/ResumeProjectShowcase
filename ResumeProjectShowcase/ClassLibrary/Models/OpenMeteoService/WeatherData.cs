namespace ClassLibrary.Models.OpenMeteoService
{
    public record WeatherData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = null!;
        public string TimezoneAbbreviation { get; set; } = null!;
        public int UtcOffsetSeconds { get; set; }
        public HourlyWeatherDataUnits HourlyWeatherDataUnits { get; set; } = null!;
        public HourlyWeatherDataLists HourlyWeatherDataLists { get; set; } = null!;
    }
}
