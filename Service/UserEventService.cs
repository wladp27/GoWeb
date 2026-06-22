using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Service
{
    public class UserEventService : IUserEventService
    {
        private readonly IUserEvent userEventRepository;
        private readonly IUserService userService;
        private readonly IMemoryCache cache;
        private readonly IEventService eventService;
        private readonly IEventRepository eventRepository;
        private readonly IRatingRepository ratingRepository;
        private readonly IMapper mapper;


        private static readonly SemaphoreSlim semofor = new SemaphoreSlim(1, 1);
        public UserEventService(IRatingRepository ratingRepository,IUserEvent userEventService, IUserService userService, IMemoryCache cache, IUserRepository userRepository, IEventService eventService, IEventRepository eventRepository, IMapper mapper) 
        {
            this.eventRepository = eventRepository;
            this.mapper = mapper;
            this.eventService = eventService;
            this.userEventRepository = userEventRepository;
            this.userEventRepository= userEventService;
            this.userService= userService;
            this.cache= cache;
            this.ratingRepository = ratingRepository;
        }
        public async Task<List<Event>> GetAllAttendedEventsUserAsync(string idUser)
        {
           return await userEventRepository.GetAllAttendedEventsUserAsync(idUser);
        }

        public async Task<List<User>> GetRegisteredUsersAsync(int idEvent)
        {
            return await userEventRepository.GetRegisteredUsersAsync(idEvent);
        }

        public async Task<List<UserEvent>> GetUsersRegistAndReservAsync(int idEvent)
        {
            return await userEventRepository.GetUsersRegistAndReservAsync(idEvent);
        }

        public async Task<Dictionary<JoiningStatus, List<User>>> GetUsersRegistAndReservDictionaryAsync(int idEvent)
        {
           return await userEventRepository.GetUsersRegistAndReservDictionaryAsync(idEvent);
        }

        public async Task<JoinResult> JoinAsync(string idUser, int idEvent)
        {
            var result = await userEventRepository.JoinAsync(idUser, idEvent);
            var succesResult = new JoinResult[] { JoinResult.SuccessNewRegistration, JoinResult.SuccessInReserve, JoinResult.SuccessStatusUpdated };
            if(succesResult.Contains(result))
            {
                cache.Remove(new UsersInEventCacheKey(idEvent));
                cache.Remove(new EventCacheKey(idEvent));
            }
            return result;  
        }

        public async Task<LeaveResult> LeaveUserAsync(string idUser, int idEvent)
        {
            var result = await userEventRepository.LeaveUserAsync(idUser, idEvent);
            if (result==LeaveResult.SuccessLeave)
            {
                cache.Remove(new UsersInEventCacheKey(idEvent));
                cache.Remove(new EventCacheKey(idEvent));
            }
            return result;
        }

        public async Task<List<UserPreviewView>> GetUsersEventAsync(int idEvent)
        {

            if (cache.TryGetValue(new UsersInEventCacheKey(idEvent), out List<string>? idUsers))
            {
                var users = await userService.GetPreviewUsers(idUsers);
                return users;
            }
            await semofor.WaitAsync();
            try
            {
                if (cache.TryGetValue(new UsersInEventCacheKey(idEvent), out idUsers))
                {
                    var users = await userService.GetPreviewUsers(idUsers);
                    return users;
                }
                var listIdUsersInEvent = await userService.GetIdUsersDB(idEvent);
                if (listIdUsersInEvent != null)
                {
                    cache.Set(new UsersInEventCacheKey(idEvent), listIdUsersInEvent, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
                    var users = await userService.GetPreviewUsers(listIdUsersInEvent);
                    return users;
                }
                cache.Set(new UsersInEventCacheKey(idEvent), listIdUsersInEvent, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60))); // если null
            }
            finally
            {
                semofor.Release();
            }
            return null;

        }

        /// <summary>
        /// Данный метод использует два запроса из-за дублирования строк события на каждую строку пользователя
        /// </summary>
        public async Task<EventUsersViewModel> GetEventsWithUserAsync(int idEvent)
        {
            IQueryable<Event> quaryable = eventRepository.GetAllEventsQueryable();
            var ev = mapper.Map<EventUsersViewModel>(await eventService.GetByIdAsync(idEvent));
            var users = await GetUsersEventAsync(idEvent);
            ev.UsersRegistered = users.Take(ev.MaxParticipants).ToList();
            ev.UsersInReserve = users.Skip(ev.MaxParticipants).ToList();
            return ev;
        }
        public record UsersInEventCacheKey(int idEvent);
    }
}
