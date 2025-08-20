using NHA.Website.Software.Entities.Game;

namespace NHA.Website.Software.Views.Game.Vms;
public class GenreIndexViewModel
{
    public string Genre { get; set; } = string.Empty;
    public List<GamePage> GameList { get; set; } = [];
}
