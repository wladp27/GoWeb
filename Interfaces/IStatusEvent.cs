using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IStatusEvent
    {
        public Task<StatusEvent> GetByIdAsync(int id);
        public Task<List<StatusEvent>> GetAllAsync();
    }
}
