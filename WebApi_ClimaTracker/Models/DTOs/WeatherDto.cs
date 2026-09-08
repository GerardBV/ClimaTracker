namespace WebApi_ClimaTracker.Models.DTOs
{
    public class WeatherDto
    {
        public string City { get; set; }
        public double CurrentTemp { get; set; }
        public double FeelsLike { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public string Condition { get; set; }
        public List<HourlyDto> Hourly { get; set; } = new();
        public List<DailyDto> Daily { get; set; } = new();

        public class HourlyDto
        {
            public string Time { get; set; }
            public double Temperature { get; set; }
        }

        public class DailyDto
        {
            public string Date { get; set; }
            public double MaxTemp { get; set; }
            public double MinTemp { get; set; }
        }
    }
}
