namespace NHA.Website.Software.Views.ViewModels;

public class AdminUsersViewModel
{
    public List<AdminUserOverviewViewModel> Users { get; set; } = new();
}

public class AdminUserOverviewViewModel
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? ProfilePicturePath { get; set; }

    public DateTime DateJoined { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];

    public IReadOnlyList<string> Permissions { get; set; } = [];
}
