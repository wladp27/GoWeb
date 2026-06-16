using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class UserRegisterViewModel
    {
        [Display(Name = "Никнейм")]
        [Required(ErrorMessage = "Пожалуйста, введите ваш никнейм")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Никнейм может содержать только латинские буквы и цифры без пробелов")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 20 символов")]
        public string UserName { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Пожалуйста, введите ваше имя")]
        [RegularExpression(@"^[a-zA-Z0-9а-яА-Я]+$", ErrorMessage = "Имя может содержать только буквы и цифры без пробелов")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 20 символов")]
        public string DisplayName { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessage = "Пожалуйста,выберите ваш город")]
        public int IdCity { get; set; }


        [Display(Name = "Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH}")]
        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите вашу почту")]
        [Display(Name = "Почта")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Пароль должен содержать: 1 заглавную, 1 цифру, 1 спец. символ.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} символов.", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите подтверждение пароля")]
        [Display(Name = "Подверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }


        public List<SelectListItem>? Cities { get; set; }
    }
}
