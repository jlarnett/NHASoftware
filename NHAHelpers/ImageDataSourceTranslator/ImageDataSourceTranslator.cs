namespace NHA.Helpers.ImageDataSourceTranslator
{
    public class ImageDataSourceTranslator : IImageDataSourceTranslator
    {
        public string GetDataSourceTranslation(string fileExtension, byte[] fileBytes)
        {
            var dataSourceMime = DetermineDataSourceMime(fileExtension);
            return dataSourceMime + Convert.ToBase64String(fileBytes);
        }

        private string DetermineDataSourceMime(string fileExtension)
        {
            var normalizedExtension = fileExtension.ToUpperInvariant();
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
                case ".MP4":
                    return "data:video/mp4;base64,";
                case ".WEBM":
                    return "data:video/webm;base64,";
                case ".OGG":
                    return "data:video/ogg;base64,";
                case ".MOV":
                    return "data:video/quicktime;base64,";
            }

            return "";
        }
    }
}
