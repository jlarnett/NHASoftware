using System.ComponentModel.DataAnnotations;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Social_Entities;
namespace NHA.Website.Software.ConsumableEntities.DTOs;

public class ReportedPostDTO
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
