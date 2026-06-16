using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Logging;

namespace GoWeb.Interfaces
{
    public interface IEventRepository
    {
        public Task<Event> GetByIdAsync(int id);

        public Task<Event> GetByIdNoTrackingAsync(int id);
        public Task<int> AddAsync(Event ev);
        public Task Update(Event ev);
        public Task DeleteAsync(Event ev);

      
        public Task<bool> DeleteBuIdAsync(int idEvent);
        public Task<List<Event>> GetAllAsync();
        public IQueryable<Event> GetAllEventsQueryable();

        public Task<Event?> GetEventWithLocationAsync(int id);
        public Task<bool> UpdateStatusEvent(int idEvent, StatusEventConts status);

        public Task<bool> ExistenceEvent(int idEvent);

        public Task<bool> CheckingCountUserAndStatus(int idEvent);

        public Task<bool> CheckingExistence(int idEvent);
    }
}
