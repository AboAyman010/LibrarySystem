using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Reader
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        // علاقة بالاستعارات
        public ICollection<Loan> Loans { get; set; }
    }
}
