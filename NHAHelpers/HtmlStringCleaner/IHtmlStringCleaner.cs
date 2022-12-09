namespace NHAHelpers.HtmlStringCleaner;

public interface IHtmlStringCleaner
{
    string Clean(string input);
    HtmlStringCleaner ConvertNewLinesToHtml();
    HtmlStringCleaner FixDoubleQuoteEscapeCharactersForHtml();
    HtmlStringCleaner initialize(string input);
}
