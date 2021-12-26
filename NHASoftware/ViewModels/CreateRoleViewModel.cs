using System.ComponentModel.DataAnnotations;

namespace NHASoftware.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
