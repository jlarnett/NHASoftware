namespace NHA.Helpers.ImageDataSourceTranslator
{
    public class ImageDataSourceTranslator : IImageDataSourceTranslator
    {
        public string GetDataSourceTranslation(string imageExtension, byte[] imageBytes)
        {
            var dataSourceMime = DetermineDataSourceMime(imageExtension);
            return dataSourceMime + Convert.ToBase64String(imageBytes);
        }

        private string DetermineDataSourceMime(string imageExtension)
        {
            var normalizedExtension = imageExtension.ToUpper();
            switch (normalizedExtension)
            {
                case ".PNG":
                    return "data:image/png;base64,";
                case ".JPG":
                    return "data:image/jpg;base64,";
                case ".JPEG":
                    return "data:image/jpeg;base64,";
                case ".BMP":
                    return "data:image/bmp;base64,";
            }

            return "";
        }
    }
}
