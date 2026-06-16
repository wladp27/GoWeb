using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class Rating
    {
        public int Value { get; set; }
        public int EventTypeId { get; set; }
        public string UserName { get; set; }
        public EventType EventType { get; set; }
        public User User { get; set; }

    }
}
