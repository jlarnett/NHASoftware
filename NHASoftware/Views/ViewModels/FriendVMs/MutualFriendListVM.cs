using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Views.ViewModels.FriendVMs;
public class MutualFriendListVM
{
    public ApplicationUser User { get; set; }
    public IEnumerable<ApplicationUser> MutualFriendsList { get; set; }

    public MutualFriendListVM(ApplicationUser user, IEnumerable<ApplicationUser> mutualfriendsList)
    {
        this.User = user;
        this.MutualFriendsList = mutualfriendsList;
    }
}
