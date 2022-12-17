using System.ComponentModel.DataAnnotations;
using NHASoftware.Entities.Identity;
using NHASoftware.Entities.Social_Entities;

namespace NHASoftware.ConsumableEntities.DTOs
{
    public class PostDTO
    {
        public int? Id { get; set; }

        [Required]
        public string Summary { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? ParentPostId { get; set; }
        public Post? ParentPost { get; set; }
    }
}
