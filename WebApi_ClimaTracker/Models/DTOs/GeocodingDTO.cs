namespace WebApi_ClimaTracker.DTOs
{
    // Réponse du géocodage (nom de ville → coordonnées)
    public class GeocodingDTO
    {
        public List<GeocodingResult>? Results { get; set; }
    }

    public class GeocodingResult
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; }
    }

    // Réponse de l'API météo
    public class OpenMeteoResponse
    {
        public CurrentWeather Current { get; set; }
        public HourlyWeather Hourly { get; set; }
        public DailyWeather Daily { get; set; }
    }

    public class CurrentWeather
    {
        public double Temperature_2m { get; set; }
        public double Apparent_temperature { get; set; }
        public int Weather_code { get; set; }
    }

    public class HourlyWeather
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature_2m { get; set; } = new();
    }

    public class DailyWeather
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature_2m_max { get; set; } = new();
        public List<double> Temperature_2m_min { get; set; } = new();
        public List<string> Sunrise { get; set; } = new();
        public List<string> Sunset { get; set; } = new();
    }
}
