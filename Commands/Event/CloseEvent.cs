using GoWeb.Interfaces;
using GoWeb.Service;
using GoWeb.Сonstants;
using MediatR;

namespace GoWeb.Commands.Event
{

     
    public record CloseEventCommand(int idEvent) : IRequest;

    public class CloseEventHandler : IRequestHandler<CloseEventCommand>
    {
        private readonly IEventService eventService;
        private readonly ICommandQueue commandQueue;
        private readonly ILogger<CloseEventHandler> logger;
        public CloseEventHandler(IEventService eventService, ICommandQueue commandQueue, ILogger<CloseEventHandler> logger)
        {
            this.eventService = eventService;
            this.commandQueue = commandQueue;
            this.logger = logger;
        }

        public async Task Handle(CloseEventCommand request, CancellationToken cancellationToken)
        {
            
            var ev = await eventService.GetByIdAsync(request.idEvent); // лишние данные 
            if (ev.StatusEventId ==(int)StatusEventConts.ReСreation)
            {
                
                var result = await eventService.UpdateStatusEvent(request.idEvent, StatusEventConts.ReСreation);
                if(result)
                {
                    var timeRecreate = ev.EndTime.AddHours((double)Timings.RecreateTime);
                    commandQueue.Enqueue(new RecreateEventCommand(request.idEvent), timeRecreate);
                    logger.LogInformation("Запланировано пересоздание события с id:{id} на {time}", request.idEvent, timeRecreate.ToLocalTime());
                }
                    
                else
                    logger.LogInformation("Обновление статуса события с {id} на статус: Пересоздание завершилось с ошибкой на", request.idEvent);
            }
            else if(ev.StatusEventId == (int)StatusEventConts.Published)
            {
                await eventService.UpdateStatusEvent(request.idEvent, StatusEventConts.Completed);
                logger.LogInformation("Изменен статус id:{id} события на завершен", request.idEvent);
            }
            else if (ev.StatusEventId == (int)StatusEventConts.Completed)
            {
                logger.LogInformation("Событие с id:{id} уже было завершено", request.idEvent);
            }
        }
    }
}


