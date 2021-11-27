namespace NHASoftware.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        //Nav Property
        public virtual IList<AuthorBook> AuthorBooks { get; set; }

        public Author()
        {
            this.AuthorBooks = new List<AuthorBook>();
        }
    }
}
