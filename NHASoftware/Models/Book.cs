using System.ComponentModel;

namespace NHASoftware.Models
{
    public class Book
    {
        public int Id { get; set; }
        [DisplayName("Book Title")]
        public string Title { get; set; }
        [DisplayName("Book Description")]
        public string Description { get; set; }

        //Nav Property
        public virtual IList<BookAuthor> BookAuthors { get; set; }

        public Book()
        {
            BookAuthors = new List<BookAuthor>();
        }
    }
}
