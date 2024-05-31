namespace ClassLibrary.Models.OpenMeteoService
{
    public class HourlyData
    {
        public long Time { get; set; }
        public long TimeEnd { get; set; }
        public int Interval { get; set; }
        public List<WeatherVariable> Variables { get; set; } = null!;
    }
}
