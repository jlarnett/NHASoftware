namespace NHA.Website.Software.Entities.Sponsors
{
    public class SponsorAd
    {
        public int Id { get; set; }
        public string? VideoUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? AdRedirectUrl { get; set; }
        public int Views { get; set; } = 0;
    }
}
