using NHASoftware.Models;

namespace NHASoftware.ViewModels
{
    public class AddBookAuthorViewModel
    {
        public int BookId { get; set; }
        public List<Author> Authors { get; set; }
        public List<Author>? SelectedAuthors { get; set; }

        public AddBookAuthorViewModel(int id, List<Author> authors, List<Author> selectedAuthors)
        {
            BookId = id;
            Authors = authors;
            SelectedAuthors = selectedAuthors;
        }
    }
}
