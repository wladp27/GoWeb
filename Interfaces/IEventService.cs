using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using System.Security.Claims;

namespace GoWeb.Interfaces
{
    public interface IEventService
    {
      public Task<List<EventSummaryViewModel>?> GetFilteredEventsAsync(EventFilterViewModel filter);
        public Task<List<CommandViewModel>> GetCommandChekingCanckeledEventAsync();
        public Task<List<CommandViewModel>> GetCommandRecreateEventAsync();
        public Task<int> AddAsync(Event ev);
        public Task<EventSummaryViewModel?> GetByIdAsync(int id);
        public Task<bool> ExistenceEvent(int idEvent);
        public Task<bool> UpdateStatusEvent(int idEvent, StatusEventConts status);
        public Task<bool> CheckingCountUserAndStatus(int idEvent);
        public Task<bool> DeleteBuIdAsync(int idEvent);
        public Task<EventIndexViewModel> GetFilterEvents(int? selectedCity, int? selectedTypeEvent);
    }
}
