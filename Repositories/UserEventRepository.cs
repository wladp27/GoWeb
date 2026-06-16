using GoWeb.Interfaces;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Repositories
{
    public class UserEventRepository : IUserEvent
    {
        private readonly IEventService eventService ;
        private readonly IUserRepository userRepository;
        private readonly ApplicationDbContext context;
        public UserEventRepository(IEventService eventService, IUserRepository userRepository, ApplicationDbContext context) 
        {
            this.eventService = eventService;
            this.userRepository = userRepository;
            this.context = context;
        }

        public async Task<List<Event>> GetAllAttendedEventsUserAsync(string idUser)
        {
           var events = await context.UsersEvents.Where(ue => ue.UserId == idUser && ue.StatusJoiningId != (int)JoiningStatus.Cancelled)
                                                 .Include(ue=>ue.Event)
                                                    .ThenInclude(e => e.Location)
                                                 .Select(ue=>ue.Event)
                                                 .ToListAsync();
            return events;
        }

        public async Task<List<User>> GetRegisteredUsersAsync(int idEvent)
        { 
           var user= await context.UsersEvents.Where(ue=>ue.EventId == idEvent && ue.StatusJoiningId == (int)JoiningStatus.Registered)
                                                   .Select(ue=>ue.User)
                                                   .ToListAsync();
            return user;
        }

        public async Task<List<UserEvent>> GetUsersRegistAndReservAsync(int idEvent)
        {
            var usersEvent = await context.UsersEvents.Where(ue => ue.EventId == idEvent &&
                                                      (ue.StatusJoiningId == (int)JoiningStatus.Registered) || ue.StatusJoiningId == (int)JoiningStatus.InReserve)
                                                      .ToListAsync();
            return usersEvent;
        }

        public async Task<Dictionary<JoiningStatus,List<User>>> GetUsersRegistAndReservDictionaryAsync(int idEvent)
        {
            var usersEvent = await context.UsersEvents.Include(ue => ue.User)
                                                      .Where(ue => ue.EventId == idEvent &&
                                                      (ue.StatusJoiningId == (int)JoiningStatus.Registered || ue.StatusJoiningId == (int)JoiningStatus.InReserve))
                                                      .ToListAsync();
            var dictUser=usersEvent.GroupBy(ue => (JoiningStatus)ue.StatusJoiningId)
                                   .ToDictionary(ue => ue.Key, ue => ue.OrderBy(ue=>ue.TimeJoinEvent).Select(a => a.User).ToList());
            return dictUser;
        }

        public async Task<JoinResult> JoinAsync(string idUser,int idEvent)
        {
            var ExistenceUser = await userRepository.ExistenceUser(idUser);
            var ev = await eventService.GetByIdAsync(idEvent);
            if (ExistenceUser)
                return JoinResult.UserNotFound;
            if (ev == null)
                return JoinResult.EventNotFound;
            if (ev.StatusEventId == (int)StatusEventConts.Published)
            {
                var timeCoincidences = await context.UsersEvents.AnyAsync(u => u.UserId == idUser && u.EventId != ev.Id 
                                                && u.Event.StatusEventId == (int)StatusEventConts.Published &&
                                                (
                                                 (ev.StartTime < u.Event.EndTime && ev.EndTime > u.Event.StartTime)
                                                )
                                        );
                if(timeCoincidences)
                {
                    return JoinResult.TimeCoincidences;
                }
                var ue = await context.UsersEvents.FirstOrDefaultAsync(ue => ue.UserId == idUser && ue.EventId == idEvent);
                var countUsersInEvent = await context.UsersEvents.Where(ue => ue.EventId == idEvent && ue.StatusJoiningId == (int)JoiningStatus.Registered).CountAsync();
                if (ue != null)
                {
                    if (ue.StatusJoiningId == (int)JoiningStatus.Cancelled)
                    {
                        if (countUsersInEvent < ev.MaxParticipants)
                        {
                            ue.StatusJoiningId = (int)JoiningStatus.Registered;
                            ue.TimeJoinEvent = DateTime.UtcNow;
                            await context.SaveChangesAsync();
                            return JoinResult.SuccessStatusUpdated;
                        }
                        else
                        {
                            ue.StatusJoiningId = (int)JoiningStatus.InReserve;
                            ue.TimeJoinEvent = DateTime.UtcNow;
                            await context.SaveChangesAsync();
                            return JoinResult.SuccessInReserve;
                        }
                    }
                    else
                        return JoinResult.AlreadyRegistered;
                }
                else 
                {
                    var userEvent = new UserEvent()
                    {
                        EventId = idEvent,
                        UserId = idUser,
                        TimeJoinEvent = DateTime.UtcNow
                    };
                    if (countUsersInEvent < ev.MaxParticipants)
                    {
                        userEvent.StatusJoiningId = (int)JoiningStatus.Registered;
                        await context.UsersEvents.AddAsync(userEvent);
                        await context.SaveChangesAsync();
                        return JoinResult.SuccessNewRegistration;
                    }
                    else 
                    {
                        userEvent.StatusJoiningId = (int)JoiningStatus.InReserve;
                        await context.UsersEvents.AddAsync(userEvent);
                        await context.SaveChangesAsync();
                        return JoinResult.SuccessInReserve;
                    }
                }         
            }
            return JoinResult.NoAccessToEvent;
        }

        public async Task<LeaveResult> LeaveUserAsync(string idUser, int idEvent)
        {
            var ExistenceUser = await userRepository.ExistenceUser(idUser);
            if (ExistenceUser)
                return LeaveResult.UserNotFound;
            var ev = await eventService.GetByIdAsync(idEvent); // лишние данные
            if (ev == null)
            {
                return LeaveResult.EventNotFound;
            }
            if(ev.EndTime<DateTimeOffset.Now)
            {
                return LeaveResult.EventIsOver;
            }

            var usersEvent= await context.UsersEvents.Where(ue => ue.EventId == idEvent).ToListAsync();
            var userInEvent = usersEvent.FirstOrDefault(u => u.UserId == idUser);
            if (userInEvent != null)
            {
                if (ev.StartTime - DateTimeOffset.Now >= TimeSpan.FromHours((double)Timings.PossibleDischargeTime))
                {
                    if (userInEvent.StatusJoiningId == (int)JoiningStatus.Registered)
                    {
                        userInEvent.StatusJoiningId = (int)JoiningStatus.Cancelled;
                        userInEvent.TimeJoinEvent = DateTime.UtcNow;
                        var userFirstInReserv = usersEvent.Where(ue => ue.StatusJoiningId == (int)JoiningStatus.InReserve).MinBy(ue => ue.TimeJoinEvent);
                        if (userFirstInReserv != null)
                            userFirstInReserv.StatusJoiningId = (int)JoiningStatus.Registered;
                        await context.SaveChangesAsync();
                        return LeaveResult.SuccessLeave;
                    }
                    else if (userInEvent.StatusJoiningId == (int)JoiningStatus.InReserve)
                    {
                        userInEvent.StatusJoiningId = (int)JoiningStatus.Cancelled;
                        userInEvent.TimeJoinEvent = DateTime.UtcNow;
                        await context.SaveChangesAsync();
                        return LeaveResult.SuccessLeave;
                    }
                    else
                        return LeaveResult.AlreadyLeave;
                }
                return LeaveResult.EvenWillStartSoon;
            }
            return LeaveResult.UserIsNotRegistered;
        }
    }
}
