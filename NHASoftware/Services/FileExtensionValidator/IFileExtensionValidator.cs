namespace NHASoftware.Services.FileExtensionValidator
{
    public interface IFileExtensionValidator
    {
        bool CheckValidImageExtensions(string uploadedFileName);
    }
}