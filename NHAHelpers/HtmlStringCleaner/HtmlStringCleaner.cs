using System.Text.RegularExpressions;

namespace NHA.Helpers.HtmlStringCleaner;

public class HtmlStringCleaner : IHtmlStringCleaner
{
    public string output { get; set; } = string.Empty;

    public HtmlStringCleaner initialize(string input)
    {
        output = input;
        return this;
    }

    public string Clean(string input)
    {
        output = input;
        return ConvertNewLinesToHtml().FixDoubleQuoteEscapeCharactersForHtml().ToString();
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
