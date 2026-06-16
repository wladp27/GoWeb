using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class UserEvent
    {
        public string UserId { get; set; }
        public int EventId { get; set; }
        public DateTimeOffset TimeJoinEvent { get; set; } // переименовать название, на время смены статуса
        public int StatusJoiningId { get; set; }
      

        public Event Event { get; set; }
        public User User { get; set; }
        public StatusJoining StatusJoining { get; set; }
    }
}
