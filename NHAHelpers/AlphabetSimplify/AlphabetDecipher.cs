namespace NHAHelpers.AlphabetSimplify
{
    public class AlphabetDecipher
    {
        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static char ConvertNumberToAlphabetLetter(int letterNumber)
        {
            return alphabet[letterNumber];
        }
    }
}
