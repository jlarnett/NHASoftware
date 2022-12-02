namespace NHAHelpers.HtmlStringCleaner;

public interface IHtmlStringBuilder
{
    HtmlStringCleaner ConvertNewLinesToHtml();
    HtmlStringCleaner FixDoubleQuoteEscapeCharactersForHtml();
    HtmlStringCleaner initialize(string input);
}
