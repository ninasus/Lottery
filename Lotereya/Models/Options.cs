using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lotereya.Models
{
    public class OptionsViewModel
    {
        [Required]
        public int id_option { get; set; }

        [Display(Name = "Группа")]
        public string option_group { get; set; }

        [Required]
        [Display(Name = "ID")]
        public string option_key { get; set; }

        [Display(Name = "Значение")]
        public string value { get; set; }

        [Display(Name = "Название")]
        public string option_name { get; set; }

        [Display(Name = "Тип поля")]
        public byte? field_type { get; set; }

        public OptionsViewModel()
        {
        }
    }
}
