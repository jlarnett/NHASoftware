using NHA.Website.Software.ConsumableEntities.DTOs;
namespace NHA.Website.Software.Views.Shared.Social.ViewModels;

public class MultiPostVM
{
    public MultiPostVM(List<PostDTO> posts)
    {
        Posts = posts;
    }

    public MultiPostVM(List<PostDTO> posts, Guid? uuid)
    {
        Posts = posts;
        UUID = uuid;
    }

    public List<PostDTO> Posts { get; set; } = new List<PostDTO>();
    public Guid? UUID { get; set; } = Guid.Empty;
}
