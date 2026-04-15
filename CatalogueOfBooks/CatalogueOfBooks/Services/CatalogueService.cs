using CatalogueOfBooks.Models;

namespace CatalogueOfBooks.Services
{
    public class CatalogueService
    {
        private List<Book> _books;
        private List<BorrowRecord> _borrows;
        private List<Reservation> _reservations;
        private FileStorageService _storage;

        public CatalogueService(FileStorageService storage)
        {
            _storage = storage;
            _books = _storage.LoadBooks();
            _borrows = _storage.LoadBorrows();
            _reservations = _storage.LoadReservations();
        }

        public void AddBook(Book book)
        {
            if (_books.Any(b => b.ISBN == book.ISBN))
                throw new Exception($"A book with ISBN '{book.ISBN}' already exists.");

            _books.Add(book);
            _storage.SaveBooks(_books);
        }

        public Book FindByISBN(string isbn)
        {
            return _books.FirstOrDefault(b => b.ISBN == isbn);
        }

        public void RemoveBook(string isbn)
        {
            Book book = FindByISBN(isbn);
            if (book == null)
            {
                throw new Exception("Book not found. Try a different isbn!");
            }

            bool isBorrowed = _borrows.Any(r => r.ISBN == isbn && !r.IsReturned);
            if (isBorrowed)
            {
                throw new Exception("Cannot remove: the book currently has active borrow records.");
            }

            _books.Remove(book);
            _storage.SaveBooks(_books);
        }

        public List<Book> SearchByTitle(string title)
        {
            return _books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    

        public List<Book> SearchByYear(int year)
        {
            return _books.Where(b => b.DateOfPublication.Year == year).ToList();
        }
            

        public List<Book> SearchByAuthor(string author)
        {
            return _books.Where(b => b.Authors.Any(a => a.Contains(author, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public void EditBook(string isbn, Book updated)
        {
            int index = _books.FindIndex(b => b.ISBN == isbn);
            if (index < 0) throw new Exception("Book not found.");
            _books[index] = updated;
            _storage.SaveBooks(_books);
        }

        public List<Book> GetAllSortedByYearThenTitle()
        {
            return _books.OrderBy(b => b.DateOfPublication.Year)
                  .ThenBy(b => b.Title)
                  .ToList();
        }

        public List<BorrowRecord> GetActiveBorrows()
        {
            return _borrows.Where(r => !r.IsReturned).ToList();
        }

        public List<Reservation> GetActiveReservations()
        {
            return _reservations.Where(r => !r.IsFulfilled).ToList();
        }

        public DateTime BorrowBook(string isbn, string borrower)
        {
            Book book = _books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null) throw new Exception("Book not found.");
            if (book.AvailableCopies <= 0) throw new Exception ("No copies available. Consider reserving the book.");

            bool alreadyBorrowed = _borrows.Any(r => r.ISBN == isbn
                                                   && r.BorrowerName == borrower
                                                   && !r.IsReturned);
            if (alreadyBorrowed) throw new Exception("This borrower already has an active loan for this book.");

            book.AvailableCopies--;
            var record = new BorrowRecord(isbn, book.Title, borrower);
            _borrows.Add(record);

            _storage.SaveBooks(_books);
            _storage.SaveBorrows(_borrows);
            return record.DueDate;
        }

        public void ReserveBook(string isbn, string reserver)
        {
            Book book = _books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null) throw new Exception("Book not found.");
            if (book.AvailableCopies > 0) throw new Exception("Copies are available; please borrow instead of reserving.");

            bool alreadyReserved = _reservations.Any(r => r.ISBN == isbn
                                                         && r.ReservedBy == reserver
                                                         && !r.IsFulfilled);
            if (alreadyReserved) throw new Exception("You already have a pending reservation for this book.");

            _reservations.Add(new Reservation(isbn, book.Title, reserver));
            _storage.SaveReservations(_reservations);
        }

        public string ReturnBook(string isbn, string borrowerName)
        {
            BorrowRecord record = _borrows.FirstOrDefault(r => r.ISBN == isbn
                                                              && r.BorrowerName == borrowerName
                                                              && !r.IsReturned);
            if (record == null) throw new Exception("No active borrow record found.");

            record.ReturnDate = DateTime.Today;
            double fine = record.CalculateFine();

            Book book = _books.First(b => b.ISBN == isbn);
            book.AvailableCopies++;

            Reservation pending = _reservations
                .Where(r => r.ISBN == isbn && !r.IsFulfilled)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefault();
            if (pending != null)
            {
                pending.IsFulfilled = true;
                BorrowBook(pending.ISBN, pending.ReservedBy);
            }
            _storage.SaveBooks(_books);
            _storage.SaveBorrows(_borrows);
            _storage.SaveReservations(_reservations);

            return fine > 0
                ? $"Book returned. Overdue fine: €{fine:F2}"
                : "Book returned on time. No fine.";
        }
    }
}
