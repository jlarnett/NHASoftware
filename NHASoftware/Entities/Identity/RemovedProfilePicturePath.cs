using System.ComponentModel.DataAnnotations;

namespace NHA.Website.Software.Entities.Identity
{
    public class RemovedProfilePicturePath
    {
        public int Id { get; set; }
        [Required] public string Path { get; set; }

        public RemovedProfilePicturePath(string path)
        {
            Path = path;
        }
    }
}
