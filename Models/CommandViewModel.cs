using MediatR;

namespace GoWeb.Models
{
    public class CommandViewModel
    {
        public IRequest command { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}
