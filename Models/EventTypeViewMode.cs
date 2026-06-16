using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class EventTypeViewMode
    {
        public int? Id { get; set; }


        [Required (ErrorMessage ="Пожалуйста, введите название типа события")]
        [StringLength (50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 50 символов.")]
        [Display(Name="Название типа события")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Прикрепите изображение")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Путь должен содержать от 2 до 100 символов")]
        [Display(Name = "Изображение события")]
        public string ImagePath { get; set; }
    }
}
