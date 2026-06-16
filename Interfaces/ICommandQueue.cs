using MediatR;
using System.Windows.Input;

namespace GoWeb.Interfaces
{
    public interface ICommandQueue
    {
        public void Enqueue(IRequest command, DateTimeOffset priority);
        public IRequest? Dequeue(out DateTimeOffset timeCommandPriority);

        public IRequest? Peek(out DateTimeOffset timeCommandPriority);

        public event Action HighestEventPriority;

        public int Count();
    }
}
