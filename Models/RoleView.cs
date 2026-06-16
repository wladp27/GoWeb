using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class RoleView
    {
        [Required(ErrorMessage = "Пожалуйста, введите название роли")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 15 символов.")]
        [Display(Name = "Название роли")]
        public string Name { get; set; }
        public string? Id { get; set; }
    }
}
