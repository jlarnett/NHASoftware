namespace NHA.Helpers.ImageDataSourceTranslator
{
    public interface IImageDataSourceTranslator
    {
        string GetDataSourceTranslation(string imageExtension, byte[] imageBytes);
    }
}
