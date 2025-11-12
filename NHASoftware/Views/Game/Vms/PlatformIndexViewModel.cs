using NHA.Website.Software.Entities.Game;

namespace NHA.Website.Software.Views.Game.Vms;
public class PlatformIndexViewModel
{
    public string Platform { get; set; } = string.Empty;
    public List<GamePage> GameList { get; set; } = [];
}