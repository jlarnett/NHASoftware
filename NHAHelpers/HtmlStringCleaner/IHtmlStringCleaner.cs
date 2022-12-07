namespace NHAHelpers.HtmlStringCleaner;

public interface IHtmlStringCleaner
{
    HtmlStringCleaner ConvertNewLinesToHtml();
    HtmlStringCleaner FixDoubleQuoteEscapeCharactersForHtml();
    HtmlStringCleaner initialize(string input);
}
