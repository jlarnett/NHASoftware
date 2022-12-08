using NHAHelpers.AlphabetSimplify;

namespace NHA.Testing
{
    public class AlphabetSimplifyTests
    {
        [Fact]
        public void CheckAlphabetDecipherReturnValues()
        {
            var decipherChar = AlphabetDecipher.ConvertNumberToAlphabetLetter(0);
            Assert.Equal('A', decipherChar);
        }

        [Fact]
        public void TryInputFalseNumber()
        {
            var decipherChar = AlphabetDecipher.ConvertNumberToAlphabetLetter(200000);
            Assert.Equal('?', decipherChar);
        }
    }
}