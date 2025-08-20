using NHA.Website.Software.Entities.Game;

namespace NHA.Website.Software.Views.Game.GameVms;
public class LetterGameIndexViewModel
{
    public char AlphabetLetter { get; set; }
    public List<GamePage> GameList { get; set; } = [];
}
