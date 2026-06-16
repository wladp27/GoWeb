using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class EventUsersProfile:Profile
    {
        public EventUsersProfile()
        {
            CreateMap<EventUsersViewModel, Event>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore());

            CreateMap<Event, EventUsersViewModel>()
             .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(x => x.Organizer.UserName));

            CreateMap<EventUsersViewModel, EventSummaryViewModel>();
            CreateMap<EventSummaryViewModel, EventUsersViewModel>();
        }
    }
}
