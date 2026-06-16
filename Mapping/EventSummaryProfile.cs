using AutoMapper;
using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class EventSummaryProfile:Profile
    {
        public EventSummaryProfile()
        {
            CreateMap<EventSummaryViewModel, Event>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore());

            CreateMap<Event, EventSummaryViewModel>()
             .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(x => x.Organizer.DisplayName))
             .ForMember(dest=>dest.CountRegisteredUsers, opt=> opt.MapFrom(x=>x.UserEvents.Where(u=>u.StatusJoiningId==(int)JoiningStatus.Registered).Count()));

            CreateMap<EventUsersViewModel, EventSummaryViewModel>();
            CreateMap<EventSummaryViewModel, EventUsersViewModel>();

        }
    }
}
