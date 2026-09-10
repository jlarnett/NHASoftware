namespace NHA.Helpers.AlphabetSimplify
{
    public class AlphabetDecipher
    {
        private static readonly string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZあいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン";

        public static char ConvertNumberToAlphabetLetter(int letterNumber)
        {
            if (letterNumber >= 0 && letterNumber < alphabet.Length)
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
