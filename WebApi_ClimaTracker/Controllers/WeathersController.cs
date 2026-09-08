using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi_ClimaTracker.Services;

[ApiController]
[Route("api/[controller]/[action]")]
public class WeathersController : Controller
{
    private readonly WeatherApiService _weatherApiService;

    public WeathersController(WeatherApiService weatherApiService)
    {
        _weatherApiService = weatherApiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetForecast([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("Le nom de la ville est requis.");

        var forecast = await _weatherApiService.GetForecastAsync(city);

        if (forecast == null)
            return NotFound($"Ville '{city}' introuvable.");

        return Ok(forecast);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFavorite([FromBody] string city)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var weather = await _weatherApiService.CreateFavoriteAsync(city, userId);
        return Ok(weather);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteFavorite(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = await _weatherApiService.DeleteFavoriteAsync(id, userId);

        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var favorites = await _weatherApiService.GetMyFavoritesAsync(userId);
        return Ok(favorites);
    }
}