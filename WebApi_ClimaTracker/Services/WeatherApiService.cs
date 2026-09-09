using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApi_ClimaTracker.DTOs;
using WebApi_ClimaTracker.Models;
using WebApi_ClimaTracker.Models.DTOs.WebApi_ClimaTracker.DTOs;

namespace WebApi_ClimaTracker.Services
{
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly WebApi_ClimaTrackerContext _context;

        //private static readonly Dictionary<int, string> WeatherCodeDescriptions = new()
        //{
        //    { 0, "Clear sky" },
        //    { 1, "Mainly clear" },
        //    { 2, "Partly cloudy" },
        //    { 3, "Overcast" },
        //    { 45, "Fog" },
        //    { 48, "Depositing rime fog" },
        //    { 51, "Light drizzle" },
        //    { 53, "Moderate drizzle" },
        //    { 55, "Dense drizzle" },
        //    { 61, "Slight rain" },
        //    { 63, "Moderate rain" },
        //    { 65, "Heavy rain" },
        //    { 71, "Slight snow" },
        //    { 73, "Moderate snow" },
        //    { 75, "Heavy snow" },
        //    { 80, "Rain showers" },
        //    { 95, "Thunderstorm" }
        //};

        public WeatherApiService(HttpClient httpClient, WebApi_ClimaTrackerContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<WeatherDto?> GetForecastAsync(string city)
        {
            // Étape 1 : géocodage (nom de ville → coordonnées)
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={city}";
            var geoResponse = await _httpClient.GetFromJsonAsync<GeocodingDTO>(geoUrl);

            if (geoResponse?.Results == null || geoResponse.Results.Count == 0)
                return null;

            var location = geoResponse.Results[0];

            // Étape 2 : météo (coordonnées → données)
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={location.Latitude.ToString()}" +
            $"&longitude={location.Longitude.ToString()}" +
            $"&current=temperature_2m,apparent_temperature,weather_code" +
            $"&hourly=temperature_2m" +
            $"&daily=temperature_2m_max,temperature_2m_min,sunrise,sunset" +
            $"&timezone=auto";

            var weatherResponse = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(weatherUrl);

            if (weatherResponse == null)
                return null;

            var condition = weatherResponse.Current.Weather_code.ToString();

            var hourlyList = new List<HourlyDto>();

            for (int i = 0; i < weatherResponse.Hourly.Time.Count; i++)
            {
                var hourlyEntry = new HourlyDto
                {
                    Time = weatherResponse.Hourly.Time[i],
                    Temperature = weatherResponse.Hourly.Temperature_2m[i]
                };

                hourlyList.Add(hourlyEntry);
            }

            var dailyList = new List<DailyDto>();

            for (int i = 0; i < weatherResponse.Daily.Time.Count; i++)
            {
                var dailyEntry = new DailyDto
                {
                    Date = weatherResponse.Daily.Time[i],
                    MaxTemp = weatherResponse.Daily.Temperature_2m_max[i],
                    MinTemp = weatherResponse.Daily.Temperature_2m_min[i]
                };

                dailyList.Add(dailyEntry);
            }

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
                Hourly = hourlyList,
                Daily = dailyList
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