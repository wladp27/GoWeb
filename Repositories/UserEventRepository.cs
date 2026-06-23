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
        private readonly IUserService userService;
        private readonly ApplicationDbContext context;
        public UserEventRepository(IEventService eventService, IUserService userService, ApplicationDbContext context) 
        {
            this.eventService = eventService;
            this.userService = userService;
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

       

        public async Task<LeaveResult> LeaveUserAsync(string idUser, int idEvent)
        {
            var user = await userService.GetPreviewUser(idUser);
            if (user==null)
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


        public async Task<JoinResult> JoinAsync(string idUser, int idEvent)
        {
            var ev = await eventService.GetByIdAsync(idEvent);
            if (ev == null)
                return JoinResult.EventNotFound;
            if (ev.StatusEventId != (int)StatusEventConts.Published)
                return JoinResult.NoAccessToEvent;
            var result=await context.Users
                .Where(u => u.Id == idUser)
                .Select(u => 
                            new
                                {
                                    RequiredRating = u.Ratings.Any(r => r.EventTypeId == ev.EventTypeId && r.Value >= ev.RequiredRating),
                                    BookedForAnotherTime = u.UserEvents.Any(ue =>
                                                                    ue.EventId != idEvent
                                                                    && ue.Event.StatusEventId == (int)StatusEventConts.Published
                                                                    && (ev.StartTime < ue.Event.EndTime && ev.EndTime > ue.Event.StartTime)),
                                    Registation = u.UserEvents.FirstOrDefault(ue => ue.EventId == idEvent),
                                    countUsersInEvent = context.UsersEvents.Where(ue => ue.EventId == idEvent && ue.StatusJoiningId == (int)JoiningStatus.Registered).Count()
                            }
                        )
                .FirstOrDefaultAsync();
            if (result == null)
                return JoinResult.UserNotFound;
            if (!result.RequiredRating)   return JoinResult.IsufficientlyRequiredRating;
            if (result.BookedForAnotherTime) return JoinResult.TimeCoincidences;
            bool hasFreePlaces = result.countUsersInEvent < ev.MaxParticipants;
            if (result.Registation != null)
            {
                if (result.Registation.StatusJoiningId == (int)JoiningStatus.Cancelled)
                    {
                        context.Entry(result.Registation).State = EntityState.Modified;
                        result.Registation.StatusJoiningId = hasFreePlaces?(int)JoiningStatus.Registered: (int)JoiningStatus.InReserve;
                        result.Registation.TimeJoinEvent = DateTime.UtcNow;
                        await context.SaveChangesAsync();
                        return hasFreePlaces ? JoinResult.SuccessStatusUpdated : JoinResult.SuccessInReserve;
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
                        TimeJoinEvent = DateTime.UtcNow,
                        StatusJoiningId = hasFreePlaces ? (int)JoiningStatus.Registered : (int)JoiningStatus.InReserve
                    };
                    context.UsersEvents.Add(userEvent);
                    await context.SaveChangesAsync();
                    return hasFreePlaces ? JoinResult.SuccessNewRegistration : JoinResult.SuccessInReserve; 
            }
        }
        



    }
}
