namespace MySecureBackend.WebApi.Models
{
    public class UserSettings
    {
        public string UserId { get; set; } = null!;

        public string KindVoornaam { get; set; } = null!;
        public string KindAchternaam { get; set; } = null!;
        public int KindLeeftijd { get; set; }

        public string ArtsNaam { get; set; } = null!;
        public string BehandelingType { get; set; } = null!;

        public string Behandeldatum { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}