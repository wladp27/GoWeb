using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        public string? LocationDescription { get; set; }

        


        public City City { get; set; }
        public List<PhotosLocation> PhotosLocations { get; set; }
        public List<Event> Events { get; set; }
    }
}
