namespace NHASoftware.Services.FileExtensionValidator
{
    public class FileExtensionValidator : IFileExtensionValidator
    {
        public bool CheckValidImageExtensions(string uploadedFileName)
        {
            string[] permittedExtensions = { ".jpg", ".png", ".jpeg", ".bmp" };
            var ext = Path.GetExtension(uploadedFileName).ToLowerInvariant();
            return IsExtensionPathEmptyOrIncorrect(ext, permittedExtensions);
        }

        private bool IsExtensionPathEmptyOrIncorrect(string ext, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }
    }
}
