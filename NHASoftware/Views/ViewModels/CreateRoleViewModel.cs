using System.ComponentModel.DataAnnotations;
namespace NHA.Website.Software.Views.ViewModels;
public class CreateRoleViewModel
{
    [Required]
    public string RoleName { get; set; } = string.Empty;
}
