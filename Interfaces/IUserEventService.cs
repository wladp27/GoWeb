using GoWeb.Models;

namespace GoWeb.Interfaces
{
    public interface IUserEventService: IUserEvent
    {
        public Task<List<UserPreviewView>> GetUsersEventAsync(int idEvent);
        public Task<EventUsersViewModel> GetEventsWithUserAsync(int idEvent);
        public record UsersInEventCacheKey(int idEvent);
    }
}
