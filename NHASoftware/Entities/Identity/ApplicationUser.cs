using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
namespace NHA.Website.Software.Entities.Identity;
public class ApplicationUser : IdentityUser
{
    [NotNull]
    public double UserCash { get; set; } = 0;

    [NotNull]
    public DateTime LastLoginDate { get; set; } = DateTime.Today;


    [AllowNull]
    public string? DisplayName { get; set; }

    [AllowNull]
    public string? ProfilePicturePath { get; set; }
}
