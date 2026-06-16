using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IUserEvent
    {
        public Task<List<User>> GetRegisteredUsersAsync(int idEvent);
        public Task<List<UserEvent>> GetUsersRegistAndReservAsync(int idEvent);
        public Task<Dictionary<JoiningStatus, List<User>>> GetUsersRegistAndReservDictionaryAsync(int idEvent);
        public Task<JoinResult> JoinAsync(string idUser,int idEvent);
        public Task<List<Event>> GetAllAttendedEventsUserAsync(string idUser);
        public Task<LeaveResult> LeaveUserAsync(string idUser, int idEvent);

    }
}
