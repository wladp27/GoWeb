using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class StatusJoining
    {
        public int Id { get; set; }
        public string TypeStatus { get; set; }

        public List<UserEvent> UserEvents { get; set; }
    }
}
