using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class PhotosLocation
    {
        public int Id { get; set; }
        public string PhotoPath { get; set; }
        public int LocationId { get; set; }

        public Location Location {get; set; }
    }
}
