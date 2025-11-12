using NHA.Website.Software.Entities.Anime;
namespace NHA.Website.Software.Views.Anime.Vms;

public class PlatformIndexViewModel
{
    public string Platform { get; set; } = string.Empty;
    public List<AnimePage> AnimeList { get; set; } = [];
}