using NHA.Website.Software.Entities.Anime;
namespace NHA.Website.Software.Views.ViewModels.AnimeVMs;
public class LetterIndexViewModel
{
    public char AlphabetLetter { get; set; }
    public List<AnimePage> AnimeList { get; set; } = new List<AnimePage>();
}
