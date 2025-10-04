using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public int ReaderId { get; set; }
        public Reader Reader { get; set; }

        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }

        // Property جاهزة ترجع حالة الكتاب
        public bool IsReturned => ReturnDate.HasValue;
    }
}
