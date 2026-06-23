using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class EventCreateUpdateViewModel : IValidatableObject
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название события")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 50 символов.")]
        [Display(Name = "Название события")]
        public string EventName { get; set; }

        [StringLength(4000, MinimumLength = 2, ErrorMessage = "Описание должно быть от 2 до 4000 символов.")]
        [Display(Name = "Описание события")]
        public string? EventDescription { get; set; }

        public LocationViewModel? Location { get; set; }

        public int? LocationId { get; set; }


        [Required(ErrorMessage = "Пожалуйста, введите рейтинг события")]
        [Display(Name = "Рейтинг события")]
        [Range(0,100, ErrorMessage = "Рейтинг события в пределах от 0 до 100")]
        public int RequiredRating {  get; set; }

        [Required(ErrorMessage = "Пожалуйста, дату и время начала события")]
        [Display(Name = "Дата и время начала события")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTimeOffset StartTime { get; set; }=DateTimeOffset.Now.AddMinutes(2);

        [Required(ErrorMessage = "Пожалуйста, дату и время окончания события")]
        [Display(Name = "Дата и время окончания события")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTimeOffset EndTime { get; set; } = DateTimeOffset.Now.AddMinutes(4);

        [Required(ErrorMessage = "Пожалуйста, введите минимальное количество участников")]
        [Display(Name = "Минимальное количество участников")]
        public int MinParticipants { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите максимальное количество участников")]
        [Display(Name = "Максимальное количество участников")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Пожалуйста,введите стоимость участия в событии")]
        [Display(Name = " Стоимость участия в событии")]
        public decimal Price { get; set; }

        [Display(Name = "Организатор события")]
        public string? OrganizerId { get; set; } = string.Empty;

        [Display(Name = "Тип события")]
        public int? EventTypeId { get; set; }

        
        public string? ImagePath { get; set; }

        [AllowedValues((int)StatusEventConts.Published, (int)StatusEventConts.Draft, ErrorMessage = "Выбран недопустимый ID уровня доступа.")]
        [Required(ErrorMessage = "Пожалуйста,выберите статус события")]
        [Display(Name = "Статус события")]
        public int StatusEventId { get; set; }


        [Display(Name = "Интервал пересоздания события (в днях)")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество дней должно быть больше 0")]
        public int? CountDaysRecreate { get; set; }



        public string? Address {  get; set; }

        [Required(ErrorMessage = "Пожалуйста,добавьте изображение для события")]
        [Display(Name = "Изображение превью события")]
        public IFormFile? ImageFile { get; set; }
        public List<SelectListItem>? EventTypes { get; set; }
        public List<SelectListItem>? StatusEvents { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime < StartTime)
            {
                yield return new ValidationResult("Дата и время окончания события не может быть раньше или совпадать с датой начала."
                , new[] { nameof(EndTime) });
            }
            if (MinParticipants > MaxParticipants)
            {
                yield return new ValidationResult("Максимальное количество участников не может быть меньше минимального."
                , new[] { nameof(MaxParticipants) });
            }

            if (LocationId == null)
            {
                if (Location == null || Location.LocationLatitude == default(double) || Location.LocationLongitude == default(double) 
                    || Location.Address==null || Location.CityId==null)
                    yield return new ValidationResult("Выберите локацию или создайте новую локацию", new[] { "Address" });
            }


            

        }
    }
}
