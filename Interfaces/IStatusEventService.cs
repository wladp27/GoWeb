using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IStatusEventService
    {
        public Task<StatusEvent> GetByIdAsync(int id);
        public Task<List<StatusEvent>> GetAllAsync();
    }
}
