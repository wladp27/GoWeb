using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GoWeb.Models
{
    public class EventFilterViewModel 
    {
        [Required(ErrorMessage = "Пожалуйста,выберите город")]
        [Display(Name = "Город")]
        public int? SelectedCity { get; set; }
        
        public double[]? SelectedCityCoordinate { get; set; }

        public List<SelectListItem>? Cities{ get; set; }
        [Display(Name = "Тип события")]
        public int? SelectedTypeEvent { get; set; }

        public List<SelectListItem>? TypeEvents { get; set; }



        public override bool Equals(object? obj)
        {
            return Equals(obj as EventFilterViewModel);
        }

        public virtual bool Equals(EventFilterViewModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return SelectedCity == other.SelectedCity &&
                   SelectedTypeEvent == other.SelectedTypeEvent;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(SelectedCity, SelectedTypeEvent);
        }

    }
}
