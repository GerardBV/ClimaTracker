using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi_ClimaTracker.Models;

public class WebApi_ClimaTrackerContext(DbContextOptions<WebApi_ClimaTrackerContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Weather> Weathers { get; set; } = default!;
}
