using System;

namespace MySecureBackend.WebApi.Models
{
    public class UserAvatar
    {
        public string UserId { get; set; } = null!;
        public int AvatarId { get; set; }
        public DateTime SelectedAt { get; set; } = DateTime.UtcNow;
    }
}