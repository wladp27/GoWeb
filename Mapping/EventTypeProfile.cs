using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class EventTypeProfile :Profile
    {
        public EventTypeProfile() 
        {
            CreateMap<EventType, EventTypeViewMode>().ReverseMap();
        }
    }
}
