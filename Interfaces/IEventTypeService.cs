using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IEventTypeService
    {
        public Task<EventTypeViewMode?> GetByIdAsync(int id);
        public Task<bool> AddAsync(EventTypeViewMode eventType);
        public Task<bool> Update(EventTypeViewMode eventType);
        public Task<bool> DeleteAsync(EventTypeViewMode eventType);
        public Task<List<EventTypeViewMode>?> GetAllAsync();
    }
}
