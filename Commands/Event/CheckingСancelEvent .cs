using GoWeb.Interfaces;
using GoWeb.Сonstants;
using MediatR;

namespace GoWeb.Commands.Event
{

    public record CheckingСancelEventCommand(int idEvent, DateTimeOffset timeEndEvent) : IRequest;
    public class CheckingСancelEventHandler : IRequestHandler<CheckingСancelEventCommand>
    {
        private readonly IEventService eventService;
        private readonly ICommandQueue commandQueue;
        private readonly ILogger<CheckingСancelEventHandler> logger;

        public CheckingСancelEventHandler(IEventService eventService, ICommandQueue commandQueue, ILogger<CheckingСancelEventHandler> logger)
        {
            this.eventService = eventService;
            this.commandQueue = commandQueue;
            this.logger = logger;
        }

        public async Task Handle(CheckingСancelEventCommand request, CancellationToken cancellationToken)
        {
            if (await eventService.CheckingCountUserAndStatus(request.idEvent))
            {
                commandQueue.Enqueue(new CloseEventCommand(request.idEvent), request.timeEndEvent);
                logger.LogInformation("Необходимое количество игроков собранно событие с {id} состоиться и завершиться в {time}", request.idEvent, request.timeEndEvent);
            }
            else // если событие кто-то отменил, то произойдет ещё одна отмена 
            {
                logger.LogInformation("Событие с {id} не состоиться, произодиться отмена", request.idEvent);
                var result = await eventService.UpdateStatusEvent(request.idEvent, StatusEventConts.Cancelled);
                if (result)
                    logger.LogInformation("Статус события с {id} успешно изменен на: Отменено", request.idEvent);
                else
                    logger.LogInformation("Обновление статуса события с {id} на статус: Отменено не удалось", request.idEvent);
            }
        }
    }
}
