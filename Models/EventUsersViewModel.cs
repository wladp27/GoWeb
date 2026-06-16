using GoWebApplication.Db.Models;

namespace GoWeb.Models
{
    public class EventUsersViewModel: EventSummaryViewModel
    {
 

        public List<UserPreviewView> UsersRegistered { get; set; }
        public List<UserPreviewView> UsersInReserve { get; set; }

    }
}
