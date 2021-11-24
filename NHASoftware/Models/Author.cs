namespace NHASoftware.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public Author()
        {
            this.Books = new List<Book>();
        }

        public virtual ICollection<Book> Books { get; set; }
    }
}
