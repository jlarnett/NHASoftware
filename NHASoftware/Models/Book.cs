namespace NHASoftware.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        //Nav Property
        public virtual IList<AuthorBook> AuthorBooks { get; set; }

        public Book()
        {
            AuthorBooks = new List<AuthorBook>();
        }
    }
}
