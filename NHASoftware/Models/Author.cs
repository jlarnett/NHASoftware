namespace NHASoftware.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        //Nav Property
        public virtual IList<BookAuthor> BookAuthors { get; set; }

        public Author()
        {
            this.BookAuthors = new List<BookAuthor>();
        }
    }
}
