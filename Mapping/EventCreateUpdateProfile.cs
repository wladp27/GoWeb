using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class EventCreateUpdateProfile : Profile
    {
        public EventCreateUpdateProfile() 
        {
            CreateMap<EventCreateUpdateViewModel, Event>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())     
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())  
                .ReverseMap();
        }
    }
}
