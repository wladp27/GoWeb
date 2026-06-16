
using GoWeb.Commands.Event;
using GoWeb.Interfaces;
using GoWeb.Repositories;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace GoWeb.Service
{
    public class TimedBackgroundService : BackgroundService
    {
        private readonly ILogger<TimedBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommandQueue commandQueue;
        public CancellationTokenSource priorityToken = new CancellationTokenSource();
        private object _lock = new object();

        public TimedBackgroundService(ILogger<TimedBackgroundService> logger, IServiceScopeFactory scopeFactory,
                                      ICommandQueue commandQueue)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            this.commandQueue = commandQueue;
            commandQueue.HighestEventPriority += UpdateTimeWhait;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Service начал работу.");

            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                var commandsRecreateEvent = await eventService.GetCommandRecreateEventAsync();
                foreach (var commandRecreate in commandsRecreateEvent)                                                //заполняю очередь командами 
                {
                    commandQueue.Enqueue(commandRecreate.command, commandRecreate.StartTime);
                }

                var cmdsChekingCanckeledEvent = await eventService.GetCommandChekingCanckeledEventAsync();
                foreach (var cmdChekingCanckeled in cmdsChekingCanckeledEvent)                                 //заполняю очередь командами 
                {
                    commandQueue.Enqueue(cmdChekingCanckeled.command, cmdChekingCanckeled.StartTime);
                }
            }





            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (commandQueue.Count() > 0)
                    {
                        var c = commandQueue.Peek(out DateTimeOffset timeCmd);
                        var timeDifference = (timeCmd - DateTimeOffset.Now);
                        if (timeDifference > TimeSpan.Zero)
                        {
                           
                            var localToken = priorityToken;
                            using var inkedSource = CancellationTokenSource.CreateLinkedTokenSource(localToken.Token, stoppingToken);

                            try
                            {
                                if (timeDifference > TimeSpan.FromDays(1))
                                {
                                    _logger.LogInformation("Время ожидания более 1 дня,сервис уходит в режим ожидания на 1 день");
                                    await Task.Delay(TimeSpan.FromDays(1), inkedSource.Token);
                                }

                                else
                                {
                                    await Task.Delay(timeDifference, inkedSource.Token);
                                    _logger.LogInformation("Сервис уходит в режим ожидания на {time}", timeDifference);
                                }
                                   

                            }
                            catch (OperationCanceledException)
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    throw new OperationCanceledException();
                                if (localToken.IsCancellationRequested)
                                {
                                    _logger.LogInformation("Появилось новое событие выше приоритетом, предыдущие ожидание сбрасывается");
                                    lock (_lock)
                                    {
                                        if (priorityToken == localToken)
                                        {
                                            localToken.Dispose();
                                            priorityToken = new CancellationTokenSource();
                                        }
                                    }
                                        continue;
                                }
                            }
                        }
                            using (IServiceScope scope = _scopeFactory.CreateScope())
                            {
                            var handler = scope.ServiceProvider.GetRequiredService<IMediator>();
                            var command = commandQueue.Peek(out DateTimeOffset timeCommand);
                                if (timeCommand <= DateTimeOffset.Now)
                                {
                                      _logger.LogInformation("Выполнение команды из очереди");
                                      await handler.Send(command, stoppingToken); 
                                      commandQueue.Dequeue(out DateTimeOffset t); 
                            }
                            }
                        
                    }
                    else
                    {
                            await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка выполнения. Попробуем снова через 5 секунд...");
                    await Task.Delay(5000, stoppingToken);
                }
            }
            _logger.LogInformation("Background Service завершает работу.");
        }

        public void UpdateTimeWhait()
        {
            lock (_lock)
            {
                priorityToken.Cancel();
            }
        }
            
        }
    }

