using Microsoft.EntityFrameworkCore;
using WebApi_ClimaTracker.Models;
using WebApi_ClimaTracker.Models.DTOs;

namespace WebApi_ClimaTracker.Services
{
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly WebApi_ClimaTrackerContext _context;

        public WeatherApiService(HttpClient httpClient, IConfiguration configuration, WebApi_ClimaTrackerContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
        }

        public async Task<WeatherDto?> GetForecastAsync(string city)
        {
            var clientId = _configuration["Xweather:ClientId"];
            var clientSecret = _configuration["Xweather:ClientSecret"];

            var url = $"https://data.api.xweather.com/forecasts/{city}?client_id={clientId}&client_secret={clientSecret}";

            return await _httpClient.GetFromJsonAsync<WeatherDto>(url);
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