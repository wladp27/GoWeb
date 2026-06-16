using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class City
    {
        public int Id { get; set; }
        public string NameCity { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }

        public List<Location> Locations { get; set; }
        public List<Event> Events { get; set; }
        public List<User> Users { get; set; }
    }
}
