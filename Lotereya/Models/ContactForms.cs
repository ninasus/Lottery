using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lotereya.Models
{
    public class HomeLetterViewModel
    {
        [Required]
        [Display(Name = "Ваше имя")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Ваш E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
    }
}
