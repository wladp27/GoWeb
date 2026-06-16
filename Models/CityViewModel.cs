using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class CityViewModel
    {

        public int? Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название города.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 50 символов.")]
        [Display(Name = "Название Города")]
        public string NameCity { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите широту города.")]
        [Display(Name = "Широта города")]
        public double LocationLatitude { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите долготу города.")]
        [Display(Name = "Долгота города")]
        public double LocationLongitude { get; set; }


    }
}
