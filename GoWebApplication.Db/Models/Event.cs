using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string? EventDescription { get; set; }
        //public string Address { get; set; } //не нужно
        //public double LocationLatitude { get; set; } //не нужно
        //public double LocationLongitude { get; set; } //не нужно
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string? OrganizerId { get; set; }
        public int? EventTypeId { get; set; }
        public string ImagePath { get; set; }
        public int StatusEventId { get; set; }
        public int? LocationId { get; set; }
        public int? CountDaysRecreate { get; set; } 
        public int RequiredRating { get; set; }


        public Location Location { get; set; }
        public EventType EventType { get; set; }
        public User Organizer { get; set; }
        public StatusEvent Status { get; set; }
        public List<UserEvent> UserEvents { get; set; }
        
    }
}
