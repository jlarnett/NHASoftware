namespace NHASoftware.ViewModels
{
    public class IndexPageViewModel
    {
        public int AuthorCount { get; set; }
        public int BookCount { get; set; }

        public IndexPageViewModel(int authorCount, int bookCount)
        {
            AuthorCount = authorCount;
            BookCount = bookCount;
        }
    }
}
