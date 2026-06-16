using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class User : IdentityUser<string>
    {

        public string DisplayName { get; set; }
        public double ReliabilityVisit { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? TimeDeleted { get; set; }
        public int idCity {  get; set; }


        public List<Event> Events { get; set; }
        public List<UserEvent> UserEvents { get; set; }
        public List<Follow> Followers{ get; set; }
        public List<Follow> Following { get; set; }
        public List<Rating> Ratings { get; set; }
        public City City { get; set; }


    }
}
