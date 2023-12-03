using NHA.Website.Software.ConsumableEntities.DTOs;
namespace NHA.Website.Software.Views.Home.Social.ViewModels;

public class MultiPostVM
{
    public MultiPostVM(List<PostDTO> posts)
    {
        Posts = posts;
    }

    public List<PostDTO> Posts { get; set; } = new List<PostDTO>();
}
