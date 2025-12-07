using NHA.Website.Software.Entities.Game;

namespace NHA.Website.Software.Views.Game.Vms
{
    public class IndexViewModel
    {
        public IEnumerable<GamePage> GameList { get; set; } = [];
    }
}
