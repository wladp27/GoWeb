using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class LocationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адресс события")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Адресс должен быть от 2 до 500 символов.")]
        [Display(Name = "Адресс события")]
        public string? Address { get; set; }
        
 
    

        [Required(ErrorMessage = "Пожалуйста,выберите город события")]

        [Display(Name = "Город события")]
        public int? CityId { get; set; }

        public List<SelectListItem>? Cities { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите широту")]
        [Display(Name = "Широта")]
        public double LocationLatitude { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите долготу")]
        [Display(Name = "Долгота")]
        public double LocationLongitude { get; set; }

        [Display(Name = "Описание локации")]
        public string? LocationDescription { get; set; }

        [Display(Name = "Фото локации")]
        public List<IFormFile>? imagesLocation { get; set; }

        public List<string>? imagesPaths { get; set; }
    }
}
