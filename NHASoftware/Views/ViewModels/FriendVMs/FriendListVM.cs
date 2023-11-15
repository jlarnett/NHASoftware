using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Views.ViewModels.FriendVMs;
public class FriendListVM
{
    public ApplicationUser User { get; set; }
    public IEnumerable<ApplicationUser> FriendsList { get; set; }

    public FriendListVM(ApplicationUser user, IEnumerable<ApplicationUser> friendsList)
    {
        this.User = user;
        this.FriendsList = friendsList;
    }

}
