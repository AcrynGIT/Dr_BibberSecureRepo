using System;

namespace MySecureBackend.WebApi.Models
{
    public class Highscore
    {
        public string UserId { get; set; } = null!;
        public string Score { get; set; } = null!;  
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}