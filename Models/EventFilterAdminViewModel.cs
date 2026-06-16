using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoWeb.Models
{
    public class EventFilterAdminViewModel: EventFilterViewModel
    {
        public int? SelectedTypeStatus { get; set; }

        public List<SelectListItem>? TypesStatuses{ get; set; }
    }
}
