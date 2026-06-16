using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Linq;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using MediatR;
using GoWeb.Commands.Event;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GoWeb.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GoWeb.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventTypeService eventTypeService;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILocationRepository locationRepository;
        private readonly ILogger<CheckingСancelEventHandler> logger;
        private readonly ICityService cityService;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semForFilter = new SemaphoreSlim(1, 1);
      
        public EventService(IEventRepository eventRepository, IMapper mapper, IMemoryCache cache, ICityService cityService, IEventTypeService eventTypeService,
                            ILocationRepository locationRepository, ILogger<CheckingСancelEventHandler> logger, IUserRepository userRepository)
        {
            this.eventRepository = eventRepository;
            this.mapper = mapper;
            this.cache = cache;
            this.locationRepository = locationRepository;
            this.logger = logger;
            this.cityService = cityService;
            this.eventTypeService = eventTypeService;
            this.userRepository = userRepository;
        }


        public async Task<EventIndexViewModel> GetFilterEvents(int? selectedCity, int? selectedTypeEvent)
        {
            var listCity = await cityService.GetAllAsync();
            var listTypeEvents = await eventTypeService.GetAllAsync();
            var filter = new EventFilterViewModel
            {

                Cities = listCity.Select(c => new SelectListItem
                {
                    Text = c.NameCity,
                    Value = c.Id.ToString()
                }).ToList(),
                TypeEvents = listTypeEvents.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                }).ToList(),
                SelectedCity = selectedCity,
                SelectedTypeEvent = selectedTypeEvent
            };
            return new EventIndexViewModel { Filter = filter };
        }

        public async Task<List<CommandViewModel>> GetCommandChekingCanckeledEventAsync()
        {
            IQueryable<Event> quaryable = eventRepository.GetAllEventsQueryable();
            var listEventDb = quaryable.Include(e=>e.Location).Where(e => e.StatusEventId == (int)StatusEventConts.Published).ToList();
            //writeEventsInCache(mapper.Map<List<EventSummaryViewModel>>(listEventDb));
            return listEventDb.Select(e => new CommandViewModel
            {
                command = new CheckingСancelEventCommand(e.Id, e.EndTime),
                StartTime = e.EndTime.AddHours(-(double)Timings.CanceledTime)
            }).ToList();
        }



        public async Task<List<CommandViewModel>> GetCommandRecreateEventAsync()
        {

            IQueryable<Event> quaryable = eventRepository.GetAllEventsQueryable();
            var listEventDb = quaryable.Include(e => e.Location).Where(e => e.StatusEventId == (int)StatusEventConts.ReСreation).ToList();
           // writeEventsInCache(mapper.Map<List<EventSummaryViewModel>>(listEventDb));
            return listEventDb.Select(e => new CommandViewModel
            {
                command = new RecreateEventCommand(e.Id),
                StartTime = e.EndTime.AddHours((double)Timings.RecreateTime)
            }).ToList();
        }






        public async Task<List<EventSummaryViewModel>?> GetFilteredEventsAsync(EventFilterViewModel filter)
        {
            var listEventsView = new List<EventSummaryViewModel>();
            if (filter.SelectedCity != null)
            {
                if (cache.TryGetValue(filter, out List<int>? listIdEvents))
                {
                     return await getEventsCaheAndDB(listIdEvents);
                }
                await semForFilter.WaitAsync();
                try
                {
                    if (cache.TryGetValue(filter, out listIdEvents))
                    {
                        return await getEventsCaheAndDB(listIdEvents);
                    }

                    var listIdEventsDB = await FilterEventsView(filter, eventRepository.GetAllEventsQueryable());
                    if (listIdEventsDB != null)
                    {
                        cache.Set(filter, listIdEventsDB, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
                        listEventsView = await getEventsCaheAndDB(listIdEventsDB);
                        return listEventsView;
                    }
                    cache.Set(filter, listIdEventsDB, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1))); // защита от частого перезапроса   
                }
                finally
                {
                    semForFilter.Release();
                }
            }
            return null;
        }



        public async Task<int> AddAsync(Event ev)
        {
            var idEvent = await eventRepository.AddAsync(ev); //условие на счёт если сохранение невозможно, например такое событие с данным временем уже существует
            if (ev.StatusEventId == (int)StatusEventConts.Published)
            {
                var eventView = mapper.Map<EventSummaryViewModel>(ev);
                if (ev.Location == null)
                    eventView.Location = mapper.Map<LocationViewModel>(await locationRepository.GetByIdAsync(ev.LocationId.Value));  
                var timeLive = ev.EndTime - DateTimeOffset.Now;
                if (timeLive > TimeSpan.Zero)
                {
                    cache.Set(new EventCacheKey(eventView.Id), eventView, new MemoryCacheEntryOptions().SetAbsoluteExpiration(timeLive));
                }
                RemoveCaheFilters(eventView);
            }
            return idEvent;
        }


   

        

        public async Task<EventSummaryViewModel?> GetByIdAsync(int id)
        {
            if (cache.TryGetValue(new EventCacheKey(id), out EventSummaryViewModel? ev))
            {
                return ev;
            }
            await semaphore.WaitAsync();
            try
            {
                if (cache.TryGetValue(new EventCacheKey(id), out ev))
                {

                    return ev;
                }
                var evDb = await eventRepository.GetAllEventsQueryable()
                                               .Where(e => id==e.Id && e.StatusEventId == (int)StatusEventConts.Published)
                                               .ProjectTo<EventSummaryViewModel>(mapper.ConfigurationProvider)
                                               .FirstOrDefaultAsync();
                if (evDb != null)
                {
                    ev = mapper.Map<EventSummaryViewModel>(evDb);
                    var timeLive = ev.EndTime - DateTimeOffset.Now;
                    if (timeLive > TimeSpan.Zero)
                    {
                        cache.Set(new EventCacheKey(ev.Id), ev, new MemoryCacheEntryOptions().SetAbsoluteExpiration(timeLive));
                    }
                    else
                    {
                        cache.Set(new EventCacheKey(ev.Id), ev, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
                else
                {
                    cache.Set(new EventCacheKey(id), ev, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
                }
                return ev;

            }
            finally
            {
                semaphore.Release();
            }
        }


    

        public async Task<List<int>> FilterEventsView(EventFilterViewModel filter, IQueryable<Event> queryableEvent)
        {
            if (filter.SelectedCity != null)
            {
                queryableEvent = queryableEvent.Where(e => e.Location.CityId == filter.SelectedCity.Value);
            }

            if (filter.SelectedTypeEvent != null)
            {
                queryableEvent = queryableEvent.Where(e => e.EventTypeId == filter.SelectedTypeEvent.Value);
            }

            var list = await queryableEvent.Where(e => e.StatusEventId == (int)StatusEventConts.Published).Select(e => e.Id).ToListAsync();
            return list;
        }

        //return await queryableEvent.ProjectTo<EventSummaryViewModel>(mapper.ConfigurationProvider).ToListAsync();
        public void RemoveCaheFilters(EventSummaryViewModel eventSummary)
        {

            var filterCity = new EventFilterViewModel() { SelectedCity = eventSummary.Location.CityId };
            cache.Remove(filterCity);
            var filterCityTypeEvent = new EventFilterViewModel() { SelectedCity = eventSummary.Location.CityId, SelectedTypeEvent = eventSummary.EventTypeId };
            cache.Remove(filterCityTypeEvent);
        }

   


        private async Task<List<EventSummaryViewModel>> GetPublishedEventsDbAsync(List<int> idEventsNotInCache)
        {
            var updateEvenstForCache = await eventRepository.GetAllEventsQueryable()
                                               .Where(e => idEventsNotInCache.Contains(e.Id) && e.StatusEventId==(int)StatusEventConts.Published)
                                               .ProjectTo<EventSummaryViewModel>(mapper.ConfigurationProvider)
                                               .ToListAsync();
            return updateEvenstForCache;
        }

        private void writeEventsInCache(List<EventSummaryViewModel> listEvents)
        {
            foreach (var evView in listEvents)
            {
                var timeLive = evView.EndTime - DateTimeOffset.Now;
                if (timeLive > TimeSpan.Zero)
                {
                    cache.Set(new EventCacheKey(evView.Id), evView, new MemoryCacheEntryOptions().SetAbsoluteExpiration(timeLive));
                    logger.LogInformation("Cобытие с id:{id} добавлено в кеш", evView.Id);
                }
            }
        }
        private async Task<List<EventSummaryViewModel>?> getEventsCaheAndDB(List<int>? listIdEventsDB)
        {
            if (listIdEventsDB == null)
                return null;
            var listEventsView= new List<EventSummaryViewModel>();
            var idEventsNotInCache = new List<int>();
            foreach (var idEv in listIdEventsDB)
            {
                if (cache.TryGetValue(new EventCacheKey(idEv), out EventSummaryViewModel evView))
                {
                    if (evView != null)
                        listEventsView.Add(evView);
                }
                else
                {
                    idEventsNotInCache.Add(idEv);
                }
            }
            if (idEventsNotInCache.Count > 0) // существует лимит на диапозон 
            {
                var eventsNotInCache = await GetPublishedEventsDbAsync(idEventsNotInCache);
                listEventsView.AddRange(eventsNotInCache);
                writeEventsInCache(eventsNotInCache);
            }
            return listEventsView;
        }

        public async Task<bool> ExistenceEvent(int idEvent)
        {
            return await eventRepository.ExistenceEvent(idEvent);
        }

        public async Task<bool> UpdateStatusEvent(int idEvent, StatusEventConts status) //доделать метод с кешем что бы был
        {
            if(status != StatusEventConts.Published && status != StatusEventConts.ReСreation)
            {
                cache.Remove(new EventCacheKey(idEvent));
                logger.LogInformation("Cобытие с {id} удалено из кеша после обновления статуса на {status}", idEvent, Enum.GetName(status));
            }
            return  await  eventRepository.UpdateStatusEvent(idEvent, status);
        }

        public async Task<bool> CheckingCountUserAndStatus(int idEvent)
        {
            return await eventRepository.CheckingCountUserAndStatus(idEvent);
        }

        public async Task<bool> DeleteBuIdAsync(int idEvent)
        {
            return await eventRepository.DeleteBuIdAsync(idEvent);
        }
    }

    public record EventCacheKey(int idEvent);

 
}
