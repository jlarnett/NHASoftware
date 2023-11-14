namespace NHA.Website.Software.Services.FileExtensionValidator
{
    public interface IFileExtensionValidator
    {
        bool CheckValidImageExtensions(string uploadedFileName);
    }
}