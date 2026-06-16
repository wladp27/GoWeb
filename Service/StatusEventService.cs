using GoWeb.Interfaces;
using GoWeb.Сonstants.Cache;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Service
{
    public class StatusEventService : IStatusEventService
    {
        private readonly IMemoryCache cache;
        private readonly IStatusEvent statusEventRepository;

        private static readonly SemaphoreSlim semForGetAll = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semForGetId = new SemaphoreSlim(1, 1);

        public StatusEventService(IMemoryCache cache, IStatusEvent statusEvent) 
        {
            this.cache = cache;
            this.statusEventRepository = statusEvent;
        }

        public async Task<List<StatusEvent>> GetAllAsync()
        {
            var statusesEvent = new List<StatusEvent>();
            if (cache.TryGetValue(CacheConst.allStatusesEvent,out statusesEvent))
            {
                return statusesEvent;
            }
            await semForGetAll.WaitAsync();
            try
            {
                if (cache.TryGetValue(CacheConst.allStatusesEvent, out statusesEvent))
                {
                    return statusesEvent;
                }
                statusesEvent = await statusEventRepository.GetAllAsync();
                cache.Set(CacheConst.allStatusesEvent, statusesEvent);
            }
            finally
            {
                semForGetAll.Release();
            }
            return statusesEvent;
        }

        public async Task<StatusEvent> GetByIdAsync(int id)
        {
            if (cache.TryGetValue(new StatusEventCacheKey(id), out StatusEvent statusEvent))
            {
                return statusEvent;
            }
            await semForGetId.WaitAsync();
            try
            {
                if (cache.TryGetValue(new StatusEventCacheKey(id), out  statusEvent))
                {
                    return statusEvent;
                }
                statusEvent = await statusEventRepository.GetByIdAsync(id);
                cache.Set(new StatusEventCacheKey(id), statusEvent);
            }
            finally
            {
                semForGetId.Release();
            }
            return statusEvent;
        }

       
    }

    public record StatusEventCacheKey(int id);
}
