using NHASoftware.Models.AnimeModels;

namespace NHASoftware.ViewModels.AnimeVMs
{
    public class LetterIndexViewModel
    {
        public char AlphabetLetter { get; set; }
        public List<AnimePage> AnimeList { get; set; }
    }
}
