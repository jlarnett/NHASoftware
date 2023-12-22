using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Entities.Session;
public class SessionHistoryEvent
{
    public int Id { get; set; }
    public string userId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public string LoginEventDescription { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.UtcNow;

}
