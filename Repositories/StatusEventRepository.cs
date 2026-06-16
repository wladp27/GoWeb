using GoWeb.Interfaces;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Repositories
{
    public class StatusEventRepository : IStatusEvent
    {
         private readonly ApplicationDbContext context;
        public StatusEventRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<StatusEvent>> GetAllAsync()
        {
             return await context.StatEvents.ToListAsync();
        }

        public async Task<StatusEvent> GetByIdAsync(int id)
        {
           return await context.StatEvents.FindAsync(id);
        }
    }
}
