using GoWeb.Interfaces;
using MediatR;

namespace GoWeb.Service
{
    public class CommandQueue : ICommandQueue
    {

        private readonly PriorityQueue<IRequest, DateTimeOffset> queue = new ();
        private readonly object locker = new();

        public event Action HighestEventPriority;
        public IRequest? Peek(out DateTimeOffset timeCommandPriority)
        {
            lock (locker)
            {
                if (queue.TryPeek(out IRequest command, out DateTimeOffset priority))
                {
                    timeCommandPriority = priority;
                    return command;
                }
                timeCommandPriority = default;
                return null;
            }
        }

        public IRequest? Dequeue(out DateTimeOffset timeCommandPriority)
        {
            lock (locker)
            {
                if (queue.TryDequeue(out IRequest command, out DateTimeOffset priority))
                {
                    timeCommandPriority = priority;
                    return command;
                }
                timeCommandPriority = default;
                return null;
            }
        }

        public void Enqueue(IRequest command, DateTimeOffset timeNewCommand)
        {
            lock (locker)
            {
                queue.Enqueue(command, timeNewCommand);
                Peek(out DateTimeOffset oldTime);
                if (oldTime > timeNewCommand)
                    HighestEventPriority?.Invoke();
            }
        }

        public int Count()
        {
            
                lock (locker)
                {
                    return queue.Count;
                }
        }
    }
}
