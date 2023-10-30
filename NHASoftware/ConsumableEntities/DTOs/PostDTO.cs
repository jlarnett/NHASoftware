﻿using System.ComponentModel.DataAnnotations;
using NHASoftware.Entities.Identity;
using NHASoftware.Entities.Social_Entities;

namespace NHASoftware.ConsumableEntities.DTOs
{
    public class PostDTO
    {
        public int? Id { get; set; }

        [Required]
        public string Summary { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int? ParentPostId { get; set; }
        public Post? ParentPost { get; set; }

        public int? LikeCount { get; set; }
        public int? DislikeCount { get; set; }
        public bool UserDislikedPost { get; set; }
        public bool UserLikedPost { get; set; }

        public List<IFormFile> ImagesFiles { get; set; } = new List<IFormFile>();
    }
}
