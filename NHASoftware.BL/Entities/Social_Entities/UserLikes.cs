using System.ComponentModel.DataAnnotations;
using NHASoftware.BL.Entities.Identity;

namespace NHASoftware.BL.Entities.Social_Entities
{
    public class UserLikes
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int PostId { get; set; }
        public Post? Post { get; set; }

        public bool IsDislike { get; set; } = false;
    }
}
