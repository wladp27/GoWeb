using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IEventTypeRepository
    {
        public Task<EventType?> GetByIdAsync(int id);
        public Task<int?> AddAsync(EventType eventType);
        public Task<bool> Update(EventType eventType);
        public Task<bool> DeleteAsync(EventType eventType);
        public Task<List<EventType>?> GetAllAsync();

    }
}
