using Microsoft.EntityFrameworkCore;
using WebApi_ClimaTracker.DTOs;
using WebApi_ClimaTracker.Models;
using WebApi_ClimaTracker.Models.DTOs.WebApi_ClimaTracker.DTOs;

namespace WebApi_ClimaTracker.Services
{
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly WebApi_ClimaTrackerContext _context;

        private static readonly Dictionary<int, string> WeatherCodeDescriptions = new()
        {
            { 0, "Clear sky" },
            { 1, "Mainly clear" },
            { 2, "Partly cloudy" },
            { 3, "Overcast" },
            { 45, "Fog" },
            { 48, "Depositing rime fog" },
            { 51, "Light drizzle" },
            { 53, "Moderate drizzle" },
            { 55, "Dense drizzle" },
            { 61, "Slight rain" },
            { 63, "Moderate rain" },
            { 65, "Heavy rain" },
            { 71, "Slight snow" },
            { 73, "Moderate snow" },
            { 75, "Heavy snow" },
            { 80, "Rain showers" },
            { 95, "Thunderstorm" }
        };

        public WeatherApiService(HttpClient httpClient, WebApi_ClimaTrackerContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<WeatherDto?> GetForecastAsync(string city)
        {
            // Étape 1 : géocodage (nom de ville → coordonnées)
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}";
            var geoResponse = await _httpClient.GetFromJsonAsync<GeocodingDTO>(geoUrl);

            if (geoResponse?.Results == null || geoResponse.Results.Count == 0)
                return null;

            var location = geoResponse.Results[0];

            // Étape 2 : météo (coordonnées → données)
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={location.Latitude}&longitude={location.Longitude}" +
                $"&current=temperature_2m,apparent_temperature,weather_code" +
                $"&hourly=temperature_2m" +
                $"&daily=temperature_2m_max,temperature_2m_min,sunrise,sunset" +
                $"&timezone=auto";

            var weatherResponse = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(weatherUrl);

            if (weatherResponse == null)
                return null;

            var condition = WeatherCodeDescriptions.TryGetValue(weatherResponse.Current.Weather_code, out var desc)
                ? desc
                : "Unknown";

            return new WeatherDto
            {
                City = location.Name,
                CurrentTemp = weatherResponse.Current.Temperature_2m,
                FeelsLike = weatherResponse.Current.Apparent_temperature,
                MaxTemp = weatherResponse.Daily.Temperature_2m_max.FirstOrDefault(),
                MinTemp = weatherResponse.Daily.Temperature_2m_min.FirstOrDefault(),
                Condition = condition,
                Sunrise = weatherResponse.Daily.Sunrise.FirstOrDefault(),
                Sunset = weatherResponse.Daily.Sunset.FirstOrDefault(),
                Hourly = weatherResponse.Hourly.Time
                    .Zip(weatherResponse.Hourly.Temperature_2m, (time, temp) => new HourlyDto
                    {
                        Time = time,
                        Temperature = temp
                    }).ToList(),
                Daily = weatherResponse.Daily.Time
                    .Select((date, i) => new DailyDto
                    {
                        Date = date,
                        MaxTemp = weatherResponse.Daily.Temperature_2m_max[i],
                        MinTemp = weatherResponse.Daily.Temperature_2m_min[i]
                    }).ToList()
            };
        }

        public async Task<Weather> CreateFavoriteAsync(string city, string userId)
        {
            var weather = new Weather
            {
                City = city,
                IsFavorite = true,
                UserId = userId
            };

            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();

            return weather;
        }

        public async Task<bool> DeleteFavoriteAsync(int id, string userId)
        {
            var weather = await _context.Weathers.FindAsync(id);

            if (weather == null || weather.UserId != userId)
                return false;

            _context.Weathers.Remove(weather);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Weather>> GetMyFavoritesAsync(string userId)
        {
            return await _context.Weathers
                .Where(w => w.UserId == userId && w.IsFavorite)
                .ToListAsync();
        }
    }
}