namespace CatalogueOfBooks.Models
{
    public class Reservation
    {
        public string ReservationId { get; set; }
        public string ISBN { get; set; }
        public string BookTitle { get; set; }
        public string ReservedBy { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsFulfilled { get; set; }

        public Reservation() { }

        public Reservation(string isbn, string bookTitle, string reservedBy)
        {
            ReservationId = Guid.NewGuid().ToString();
            ISBN = isbn;
            BookTitle = bookTitle;
            ReservedBy = reservedBy;
            ReservationDate = DateTime.Today;
            IsFulfilled = false;
        }

        public override string ToString()
        {
            string status = IsFulfilled ? "[FULFILLED]" : "[PENDING]";
            return $"{status} [{ISBN}] \"{BookTitle}\" - Reserved by: {ReservedBy} on {ReservationDate:dd/MM/yyyy}";
        }
    }
}
