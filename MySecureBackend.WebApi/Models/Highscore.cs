namespace MySecureBackend.WebApi.Models
{
    public class Highscore
    {
        public string UserId { get; set; } = null!;
        public int Score { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}