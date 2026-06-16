using GoWeb.Interfaces;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Repositories
{
    public class EventTypeRepository : IEventTypeRepository
    {
        private readonly ApplicationDbContext context;
        public EventTypeRepository(ApplicationDbContext context) 
        {
            this.context = context;
        }

        public async Task<int?> AddAsync(EventType eventType)
        {
            try
            {
                await context.EventTypes.AddAsync(eventType);
                var rowsAffected = await context.SaveChangesAsync();
                if (rowsAffected > 0)
                {
                    return eventType.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> DeleteAsync(EventType eventType)
        {
            try
            {
                int rowsAffected = await context.EventTypes.Where(et => et.Id == eventType.Id).ExecuteDeleteAsync();
                if (rowsAffected != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<EventType>?> GetAllAsync()
        {
           return await context.EventTypes.ToListAsync();
        }

        public async Task<EventType?> GetByIdAsync(int id)
        {
           return await context.EventTypes.FindAsync(id);
            
        }

        public async Task<bool> Update(EventType eventType)
        {
            try
            {
                context.Update(eventType);
                int rowsAffected = await context.SaveChangesAsync();
                if (rowsAffected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
