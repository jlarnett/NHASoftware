using NHASoftware.Entities.Social_Entities;

namespace NHA.Website.Software.Entities.Social_Entities
{
    public class PostImage
    {
        public int Id { get; set; }
        public byte[] ImageBytes { get; set; }
        public int? PostId { get; set; }
        public Post? Post { get; set; }
        public string FileExtensionType { get; set; }
    }
}
