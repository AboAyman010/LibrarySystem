using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models.ViewModel
{
    public class ResendEmailConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
