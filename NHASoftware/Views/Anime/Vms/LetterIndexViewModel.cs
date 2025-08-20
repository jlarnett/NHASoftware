using NHA.Website.Software.Entities.Anime;
namespace NHA.Website.Software.Views.Anime.Vms;
public class LetterIndexViewModel
{
    public char AlphabetLetter { get; set; }
    public List<AnimePage> AnimeList { get; set; } = [];
}
