using CatalogueOfBooks.Models;
using System.Text.Json;

namespace CatalogueOfBooks.Services
{
    public class FileStorageService
    {
        private readonly string _booksListFile;
        private readonly string _borrowsListFile;
        private readonly string _reservationsFile;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public FileStorageService()
        {
            var projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..")
            );

            var dataDirectory = Path.Combine(projectRoot, "CatalogueOfBooks");

            Directory.CreateDirectory(dataDirectory);
            _booksListFile = Path.Combine(dataDirectory, "books.json");
            _borrowsListFile = Path.Combine(dataDirectory, "borrows.json");
            _reservationsFile = Path.Combine(dataDirectory, "reservations.json");
        }

        public void SaveBooks(List<Book> books)
        {
            File.WriteAllText(_booksListFile, JsonSerializer.Serialize(books, _jsonOptions));
        }

        public List<Book> LoadBooks()
        {
            if (!File.Exists(_booksListFile)) return new List<Book>();
            string json = File.ReadAllText(_booksListFile);
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        public List<BorrowRecord> LoadBorrows()
        {
            if (!File.Exists(_borrowsListFile)) return new List<BorrowRecord>();
            string json = File.ReadAllText(_borrowsListFile);
            return JsonSerializer.Deserialize<List<BorrowRecord>>(json) ?? new List<BorrowRecord>();
        }

        public List<Reservation> LoadReservations()
        {
            if (!File.Exists(_reservationsFile)) return new List<Reservation>();
            string json = File.ReadAllText(_reservationsFile);
            return JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
        }

        public void SaveBorrows(List<BorrowRecord> borrows)
        {
            File.WriteAllText(_borrowsListFile, JsonSerializer.Serialize(borrows, _jsonOptions));
        }

        internal void SaveReservations(List<Reservation> reservations)
        {
            File.WriteAllText(_reservationsFile, JsonSerializer.Serialize(reservations, _jsonOptions));
        }
    }
}
