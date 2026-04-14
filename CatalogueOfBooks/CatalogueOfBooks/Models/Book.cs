namespace CatalogueOfBooks.Models
{
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime DateOfPublication { get; set; }
        public string Publisher { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
        public string Genre { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public Book() { }

        public Book(string isbn, string title, DateTime dateOfPublication,
                    string publisher, List<string> authors, string genre, int totalCopies)
        {
            ISBN = isbn;
            Title = title;
            DateOfPublication = dateOfPublication;
            Publisher = publisher;
            Authors = authors;
            Genre = genre;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
        }

        public override string ToString()
        {
            return $"[{ISBN}] \"{Title}\" by {string.Join(", ", Authors)} ({DateOfPublication.Year}) - {Publisher}";
        }
    }
}
