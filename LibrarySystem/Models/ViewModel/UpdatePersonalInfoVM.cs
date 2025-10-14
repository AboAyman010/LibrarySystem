using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models.ViewModel
{
    public class UpdatePersonalInfoVM
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        [Display(Name = "Profile Image")]
        public IFormFile ProfileImage { get; set; }
        public string ExistingImage { get; set; } // عشان نعرض الصورة الحالية

    }
}
