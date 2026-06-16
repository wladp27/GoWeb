using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class StatusEvent
    {
        public int Id { get; set; }
        public string TypeStatus { get; set; }
        public string Code { get; set; }

        public List<Event> Events { get; set; }
    }
}
