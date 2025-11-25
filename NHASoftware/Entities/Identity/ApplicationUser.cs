using System.ComponentModel.DataAnnotations;
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
    [MaxLength(20)]
    public string? DisplayName { get; set; }

    [AllowNull]
    [MaxLength(1000)]
    public string? ProfilePicturePath { get; set; }


    [AllowNull]
    [MaxLength(1000)]
    public string Biography { get; set; } = "";

    [NotNull]
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
}
