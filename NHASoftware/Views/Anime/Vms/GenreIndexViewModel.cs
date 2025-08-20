using NHA.Website.Software.Entities.Anime;
namespace NHA.Website.Software.Views.Anime.Vms;
public class GenreIndexViewModel
{
    public string Genre { get; set; } = string.Empty;
    public List<AnimePage> AnimeList { get; set; } = [];
}
