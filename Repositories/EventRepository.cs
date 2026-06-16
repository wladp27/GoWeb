using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace GoWeb.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext context;
        public EventRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<int> AddAsync(Event ev) // нет обработки ошибки добавления 
        {
            await context.Events.AddAsync(ev); // вроде медленнее работает т.к. add просто помечает объект как добвленный 
            await context.SaveChangesAsync();
            return ev.Id;
        }

        public async Task DeleteAsync(Event ev)
        {
            var entity = await context.Events.FindAsync(ev.Id);
            if (entity != null)
            {
                context.Events.Remove(entity);
            }

            await context.SaveChangesAsync();
        }


        public async Task<List<Event>> GetAllAsync()
        {
            return await context.Events.ToListAsync();
        }


        public async Task<Event> GetByIdNoTrackingAsync(int id)
        {
            return await context.Events.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Event?> GetEventWithLocationAsync(int id)
        {
            return await context.Events.AsNoTracking().Include(e=>e.Location).FirstOrDefaultAsync(b => b.Id == id);
        }


        public async Task<Event> GetByIdAsync(int id)
        {
            return await context.Events.FindAsync(id);
        }



        public IQueryable<Event> GetAllEventsQueryable()
        {
            return context.Events.AsQueryable();
        }

        public async Task Update(Event ev)
        {
            context.Update(ev);
            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBuIdAsync(int idEvent)
        {
            int affectedRows = await context.Events.Where(e => e.Id == idEvent)
                                .ExecuteDeleteAsync();
            return affectedRows > 0;
        }

        public async Task<bool> UpdateStatusEvent(int idEvent, StatusEventConts status)
        {
            var ev = await GetByIdAsync(idEvent);
            if (ev != null && ev.StatusEventId!= (int)status)
            {
                ev.StatusEventId = (int)status;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistenceEvent(int idEvent)
        {
            return await context.Events.AnyAsync(e => e.Id == idEvent);
        }

        public async Task<bool> CheckingCountUserAndStatus(int idEvent)
        {
            return await context.Events.AnyAsync(e => e.Id == idEvent && e.StatusEventId == (int)StatusEventConts.Published && e.UserEvents.Count() >= e.MinParticipants);
        }



        public async Task<bool> CheckingExistence(int idEvent)
        {
           return  await context.Events.AnyAsync(e => e.Id == idEvent);
        }
    }
}
