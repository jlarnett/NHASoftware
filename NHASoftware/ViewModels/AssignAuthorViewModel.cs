 using NHASoftware.Models;

namespace NHASoftware.ViewModels
{
    public class AssignAuthorViewModel
    {
        public int BookId { get; set; }
        public List<Author> Authors { get; set; }
        public List<Author>? SelectedAuthors { get; set; }

        public AssignAuthorViewModel(int id, List<Author> authors)
        {
            BookId = id;
            Authors = authors;
            SelectedAuthors = authors;
        }
    }
}
