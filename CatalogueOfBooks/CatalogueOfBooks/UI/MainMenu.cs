using CatalogueOfBooks.Models;
using CatalogueOfBooks.Services;

namespace CatalogueOfBooks.UI
{
    public class MainMenu
    {
        private CatalogueService _catalogue;

        public MainMenu(CatalogueService catalogue)
        {
            _catalogue = catalogue;
        }

        public void RunMenu()
        {
            Console.Clear();
            PrintBanner();

            bool running = true;

            while (running)
            {
                try
                {
                    ShowMainMenu();
                    string choice = Console.ReadLine()?.Trim() ?? "";
                    switch (choice)
                    {
                        case "1": MenuAddBook(); break;
                        case "2": MenuRemoveBook(); break;
                        case "3": MenuSearchBook(); break;
                        case "4": MenuEditBook(); break;
                        case "5": MenuSortedList(); break;
                        case "6": MenuBorrowedBooks(); break;
                        case "7": MenuReservedBooks(); break;
                        case "8": MenuBorrowBook(); break;
                        case "9": MenuReserveBook(); break;
                        case "10": MenuReturnBook(); break;
                        case "0": running = false; break;
                        default: throw new Exception("Invalid option. Please try again.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e.Message}");
                }
                finally
                {
                    Pause();
                }
            }
        }

        private void MenuAddBook()
        {
            Console.WriteLine("//  Add a New Book  \\");
            try{
                string isbn = ConsoleHelper.WriteString("ISBN");
                string title = ConsoleHelper.WriteString("Title");
                DateTime pubDate = ConsoleHelper.WriteDate("Date of Publication");
                string publisher = ConsoleHelper.WriteString("Publisher");
                var authors = ConsoleHelper.WriteList("author(s)");

                if (authors.Count == 0) throw new Exception("At least one author is required.");

                string genre = ConsoleHelper.WriteString("Genre", required: false);
                int copies = ConsoleHelper.WriteInt("Number of copies", min: 1, max: 999);

                var book = new Book(isbn, title, pubDate, publisher, authors, genre, copies);
                _catalogue.AddBook(book);
                Console.WriteLine("Book add succesfully!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuRemoveBook()
        {
            try
            {
                Console.WriteLine("Remove a Book");
                string isbn = ConsoleHelper.WriteString("ISBN of book to remove");

                Console.Write("Are you sure you want to remove it? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y") throw new Exception("Cancelled.");

                _catalogue.RemoveBook(isbn);
                Console.WriteLine("Book removed succesfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuSearchBook()
        {
            try
            {
                Console.WriteLine("### Search Books ###");
                Console.WriteLine("1 - Search by Title");
                Console.WriteLine("2 - Search by Year");
                Console.WriteLine("3 - Search by Author");
                Console.Write("Choose search type: ");
                string opt = Console.ReadLine()?.Trim() ?? "";

                List<Book> results;
                switch (opt)
                {
                    case "1":
                        string title = ConsoleHelper.WriteString("Title (partial match)");
                        results = _catalogue.SearchByTitle(title);
                        break;
                    case "2":
                        int year = ConsoleHelper.WriteInt("Year", 1000, DateTime.Today.Year);
                        results = _catalogue.SearchByYear(year);
                        break;
                    case "3":
                        string author = ConsoleHelper.WriteString("Author name (partial match)");
                        results = _catalogue.SearchByAuthor(author);
                        break;
                    default:
                        throw new Exception("Invalid option.");
                }

                PrintBookList(results, "Search Results");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        public void MenuEditBook()
        {
            try
            {
                Console.WriteLine("### Edit Book Details ###");
                string isbn = ConsoleHelper.WriteString("ISBN of book to edit");
                var existing = _catalogue.FindByISBN(isbn);
                if (existing == null) 
                {
                    throw new Exception("Book not found."); 
                }

                Console.WriteLine("If you leave field blank we will keep the current value.");
                Console.WriteLine();

                string title = ConsoleHelper.WriteString($"Title [{existing.Title}]", required: false);
                if (title == "") title = existing.Title;

                Console.Write($"  Date of Publication [{existing.DateOfPublication:dd/MM/yyyy}] (dd/MM/yyyy or blank): ");
                string dateRaw = Console.ReadLine()?.Trim() ?? "";
                DateTime pubDate = (dateRaw == "" || !DateTime.TryParseExact(dateRaw, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime pd))
                    ? existing.DateOfPublication : pd;

                string publisher = ConsoleHelper.WriteString($"Publisher [{existing.Publisher}]", required: false);
                if (publisher == "") publisher = existing.Publisher;

                Console.WriteLine($"Current authors: {string.Join(", ", existing.Authors)}");
                Console.Write("  Re-enter authors? (y/n): ");
                List<string> authors = Console.ReadLine()?.Trim().ToLower() == "y"
                    ? ConsoleHelper.WriteList("author(s)")
                    : existing.Authors;
                if (authors.Count == 0) authors = existing.Authors;

                string genre = ConsoleHelper.WriteString($"Genre [{existing.Genre}]", required: false);
                if (genre == "") genre = existing.Genre;

                int copies = ConsoleHelper.WriteInt($"Total copies [{existing.TotalCopies}]", min: 1, max: 999);

                var updated = new Book(isbn, title, pubDate, publisher, authors, genre, copies);
                _catalogue.EditBook(isbn, updated);
                Console.WriteLine("Book edited succesfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuSortedList()
        {
            try
            {
                List<Book> books = _catalogue.GetAllSortedByYearThenTitle();
                PrintBookList(books, "All Books — Sorted by Year then Title");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuBorrowedBooks()
        {
            try
            {
                Console.WriteLine("### Currently Borrowed Books ###");
                List<BorrowRecord> records = _catalogue.GetActiveBorrows();
                if (records.Count == 0) throw new Exception("No books are currently borrowed.");

                int i = 1;
                foreach (var r in records)
                {
                    Console.WriteLine($"{i++}. {r}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuReservedBooks()
        {
            try
            {
                Console.WriteLine("### Current Reservations ###");
                List<Reservation> reservations = _catalogue.GetActiveReservations();
                if (reservations.Count == 0) throw new Exception("No pending reservations.");

                int i = 1;
                foreach (var r in reservations)
                    Console.WriteLine($"{i++}. {r}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuBorrowBook()
        {
            try
            {
                Console.WriteLine("### Borrow a Book ###");
                string isbn = ConsoleHelper.WriteString("ISBN");
                string borrower = ConsoleHelper.WriteString("Your name");
                var dueDate = _catalogue.BorrowBook(isbn, borrower);
                Console.WriteLine($"Book borrowed. Due date: {dueDate:dd/MM/yyyy}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuReserveBook()
        {
            try
            {
                Console.WriteLine("### Reserve a Book ###");
                string isbn = ConsoleHelper.WriteString("ISBN");
                string reserver = ConsoleHelper.WriteString("Your name");
                _catalogue.ReserveBook(isbn, reserver);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void MenuReturnBook()
        {
            try
            {
                Console.WriteLine("### Return a Book ###");
                string isbn = ConsoleHelper.WriteString("ISBN");
                string borrower = ConsoleHelper.WriteString("Your name");
                _catalogue.ReturnBook(isbn, borrower);
                Console.WriteLine($"Book returned succesfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
            }
            finally
            {
                Pause();
            }
        }

        private void PrintBookList(List<Book> books, string heading)
        {
            Console.WriteLine(heading);
            if (books.Count == 0) 
            { 
                throw new Exception("No books found."); 
            }

            Console.WriteLine($"{"#",-4} {"ISBN",-15} {"Title",-30} {"Year",-6} {"Authors",-25} {"Avail",5}");
            Console.WriteLine(new string('─', 90));

            int i = 1;
            foreach (var b in books)
            {
                string authStr = string.Join(", ", b.Authors);
                if (authStr.Length > 24) authStr = authStr[..21] + "…";
                string titleStr = b.Title.Length > 29 ? b.Title[..26] + "…" : b.Title;
                Console.WriteLine($"{i++,-4} {b.ISBN,-15} {titleStr,-30} {b.DateOfPublication.Year,-6} {authStr,-25} {b.AvailableCopies,2}/{b.TotalCopies}");
            }
        }

        private void Pause()
        {
            Console.Write("\nPress any key to continue!");
            Console.ReadKey(true);
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            PrintBanner();

            Console.WriteLine("##  Main Menu  ##");
            Console.WriteLine("1 - Add a book to the catalogue");
            Console.WriteLine("2 - Remove a book from the catalogue");
            Console.WriteLine("3 - Search a book by title, year and/or author");
            Console.WriteLine("4 - Edit book details");
            Console.WriteLine("5 - See sorted list of all books per year and title");
            Console.WriteLine("6 - See borrowed books");
            Console.WriteLine("7 - See reserved books");
            Console.WriteLine("8 - Borrow a book");
            Console.WriteLine("9 - Reserve a book");
            Console.WriteLine("10 - Return a book");
            Console.WriteLine("0 - Quit");
            Console.WriteLine();
            Console.Write("Enter your choice: ");
        }

        private void PrintBanner()
        {
            Console.WriteLine(@"
            *********************************
            C A T A L O G U E  O F  B O O K S
            
            ");
        }
    }
}
