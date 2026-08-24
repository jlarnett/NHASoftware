namespace NHA.Helpers.ImageDataSourceTranslator
{
    public interface IImageDataSourceTranslator
    {
        string GetDataSourceTranslation(string fileExtension, byte[] fileBytes);
    }
}
