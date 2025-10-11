﻿using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models.ViewModel
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
