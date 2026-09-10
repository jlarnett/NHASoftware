using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHA.Helpers.AlphabetSimplify;

namespace NHA.Testing.AlphabetSimplifyTests
{
    [TestClass]
    public class AlphabetSimplifyTests
    {
        [TestMethod]
        [DataRow(0,'A')]
        [DataRow(1,'B')]
        [DataRow(25, 'Z')]
        [DataRow(26, '\u3042')]
        [DataRow(71, '\u3093')]
        [DataRow(72, '\u30A2')]
        [DataRow(117, '\u30F3')]
        [DataRow(5000, '?')]
        [DataRow(-45123, '?')]
        public void CheckAlphabetDecipherReturnValues(int alphabetLetterNumber, char expectedReturnAlphabetCharacter)
        {
            var decipherChar = AlphabetDecipher.ConvertNumberToAlphabetLetter(alphabetLetterNumber);
            Assert.AreEqual(expectedReturnAlphabetCharacter, decipherChar);
        }

    }
}