using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class EventType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath {get; set;}

        public List<Event> Events { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
