using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Entities.Social_Entities
{
    public class ReportedPost
    {
        public int? Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? PostId { get; set; }
        public Post? Post { get; set; }

        [MaxLength(50)]
        public required string ReasonForReport { get; set; }

        [MaxLength(1000)]
        public string? ExtraInformation { get; set; }

    }
}
