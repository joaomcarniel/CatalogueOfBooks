namespace CatalogueOfBooks.Models
{
    public class BorrowRecord
    {
        public string RecordId { get; set; }
        public string ISBN { get; set; }
        public string BookTitle { get; set; }
        public string BorrowerName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;

        private const int BORROW_DAYS = 10;
        private const double FINE_PER_DAY = 0.50;

        public BorrowRecord() { }

        public BorrowRecord(string isbn, string bookTitle, string borrowerName)
        {
            RecordId = Guid.NewGuid().ToString();
            ISBN = isbn;
            BookTitle = bookTitle;
            BorrowerName = borrowerName;
            BorrowDate = DateTime.Today;
            DueDate = DateTime.Today.AddDays(BORROW_DAYS);
        }

        public double CalculateFine()
        {
            if (IsReturned) return 0;
            int daysOverdue = (int)(DateTime.Today - DueDate).TotalDays;
            return daysOverdue > 0 ? daysOverdue * FINE_PER_DAY : 0;
        }

        public bool IsOverdue => !IsReturned && DateTime.Today > DueDate;

        public override string ToString()
        {
            string status = IsReturned ? $"Returned on {ReturnDate:dd/MM/yyyy}" :
                            IsOverdue ? $"OVERDUE (Fine: €{CalculateFine():F2})" :
                            $"Due: {DueDate:dd/MM/yyyy}";
            return $"[{ISBN}] \"{BookTitle}\" - Borrowed by: {BorrowerName} on {BorrowDate:dd/MM/yyyy} | {status}";
        }
    }
}
