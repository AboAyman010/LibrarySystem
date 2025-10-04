using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Author { get; set; }

        [Range(0, 1000)]
        public int TotalCopies { get; set; } = 1;

        [Range(0, 1000)]
        public int AvailableCopies { get; set; } = 1;
        public ICollection<Loan> Loans { get; set; }

    }
}
