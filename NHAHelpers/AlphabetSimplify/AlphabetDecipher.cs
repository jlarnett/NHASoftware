namespace NHAHelpers.AlphabetSimplify
{
    public class AlphabetDecipher
    {
        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static char ConvertNumberToAlphabetLetter(int letterNumber)
        {
            if (letterNumber >= 0 && letterNumber < 26)
            {
                return alphabet[letterNumber];
            }
            else
            {
                return '?';
            }
        }
    }
}
