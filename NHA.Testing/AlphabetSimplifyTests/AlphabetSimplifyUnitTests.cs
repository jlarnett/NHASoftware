using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHAHelpers.AlphabetSimplify;

namespace NHA.Testing.AlphabetSimplifyTests
{
    [TestClass]
    public class AlphabetSimplifyTests
    {
        [DataTestMethod]
        [DataRow(0,'A')]
        [DataRow(1,'B')]
        [DataRow(25, 'Z')]
        [DataRow(5000, '?')]
        [DataRow(-45123, '?')]
        public void CheckAlphabetDecipherReturnValues(int alphabetLetterNumber, char expectedReturnAlphabetCharacter)
        {
            var decipherChar = AlphabetDecipher.ConvertNumberToAlphabetLetter(alphabetLetterNumber);
            Assert.AreEqual(expectedReturnAlphabetCharacter, decipherChar);
        }

    }
}