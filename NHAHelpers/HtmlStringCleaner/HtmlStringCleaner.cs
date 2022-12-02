using System.Text.RegularExpressions;

namespace NHAHelpers.HtmlStringCleaner;
    public class HtmlStringCleaner : IHtmlStringBuilder
    {
        public string output { get; set; }

        public HtmlStringCleaner initialize(string input)
        {
            output = input;
            return this;
        }

        public HtmlStringCleaner ConvertNewLinesToHtml()
        {
            output = Regex.Replace(output, @"\r\n?|\n", "<br>");
            return this;
        }

        public HtmlStringCleaner FixDoubleQuoteEscapeCharactersForHtml()
        {
            output = Regex.Replace(output, @"['""]", string.Empty);
            return this;
        }

        public override string ToString()
        {
            return output;
        }
    }
