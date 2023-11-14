using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHA.Website.Software.Services.FileExtensionValidator;

namespace NHA.Testing.FileExtensionValidatorTests
{
    [TestClass]
    public class FileExtensionValidatorTests
    {
        [DataTestMethod]
        [DataRow("test.jpg", true)]
        [DataRow("test.png", true)]
        [DataRow("test.jpeg", true)]
        [DataRow("test.bmp", true)]
        [DataRow("test.pdf", false)]
        public void CheckValidImageExtensions(string fileNameInput, bool expectedResult)
        {
            IFileExtensionValidator validator = new FileExtensionValidator();
            var result = validator.CheckValidImageExtensions(fileNameInput);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
