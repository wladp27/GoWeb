using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Сonstants;
using MediatR;
using GoWebApplication.Db.Models;

namespace GoWeb.Commands.Event
{
    public record RecreateEventCommand(int idEvent) :IRequest;

    public class RecreateEventHandler : IRequestHandler<RecreateEventCommand>
    {
        private readonly IEventService eventService;
        private readonly ILogger<RecreateEventHandler> logger;
        private readonly ICommandQueue commandQueue;
        private readonly IMapper mapper;    
        public RecreateEventHandler(IEventService eventService, ILogger<RecreateEventHandler> logger, ICommandQueue commandQueue, IMapper mapper) 
        {
            this.eventService = eventService;
            this.logger = logger;
            this.commandQueue = commandQueue;
            this.mapper = mapper;
        }
        public async Task Handle(RecreateEventCommand request, CancellationToken cancellationToken)
        {
           var ev = await eventService.GetByIdAsync(request.idEvent); 
           if(ev!=null)
           {
                if(ev.StatusEventId==(int)StatusEventConts.ReСreation)
                {
                    await eventService.UpdateStatusEvent(request.idEvent, StatusEventConts.Completed);
                    var oldId = ev.Id;
                    ev.Id = 0;
                    ev.StatusEventId = (int)StatusEventConts.Published;
                    ev.StartTime = ev.StartTime.AddDays((double)ev.CountDaysRecreate);
                    ev.EndTime = ev.EndTime.AddDays((double)ev.CountDaysRecreate);
                    int countAdd = await eventService.AddAsync(mapper.Map<GoWebApplication.Db.Models.Event>(ev));
                    if (countAdd > 0)
                    {
                        logger.LogInformation("Событие с id:{oldId} пересоздано на новое событие с id:{newId}", oldId, ev.Id);
                        commandQueue.Enqueue(new CloseEventCommand(ev.Id), ev.EndTime);
                        logger.LogInformation("Событие с id:{newId} будет завершено в {time}", ev.Id, ev.EndTime.ToLocalTime());
                    }
                    else
                    {
                        logger.LogInformation("Ошибка добвления нового события при пересоздании в БД");
                    }
                }
                else
                {
                    logger.LogInformation("Не удалось пересоздать событие,статус события с id {id} не равен Запланировано", ev.Id);
                }
            }
            else
            {
                logger.LogInformation("Не удалось пересоздать событие, события с таким id {id} не существует", ev.Id);
            }

        }
    }
}
