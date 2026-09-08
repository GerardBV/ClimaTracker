namespace WebApi_ClimaTracker.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public string City { get; set; }
        public bool IsFavorite { get; set; }
        public string UserId { get; set; }
    }
}
