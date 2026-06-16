using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Сonstants.Cache;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Service
{
    public class CityService : ICityService
    {
        private readonly ICityRepository cityRepository;
        private readonly IMemoryCache cache;
        private readonly IMapper mapper;
        private static readonly SemaphoreSlim semForGetAll = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semForGetId = new SemaphoreSlim(1, 1);
        public CityService(ICityRepository cityRepository, IMemoryCache cache, IMapper mapper)
        {
            this.cityRepository = cityRepository;
            this.cache = cache;
            this.mapper = mapper;
        }

        public async Task<bool> AddAsync(CityViewModel cityView)
        {
            cityView.Id = await cityRepository.AddAsync(mapper.Map<City>(cityView));
            if (cityView.Id != null)
            {
                cache.Set(new CityCacheKey(cityView.Id.Value), cityView);
                return true;
            }
            return false;

        }

        public async Task<bool> DeleteAsync(CityViewModel city)
        {

            var successDelete = await cityRepository.DeleteAsync((mapper.Map<City>(city)));
            if (successDelete)
            {
                cache.Remove(new CityCacheKey(city.Id.Value));
                return true;
            }
            return false;
        }

        public async Task<List<CityViewModel>?> GetAllAsync()
        {
            var allCityView = new List<CityViewModel>();
            if (cache.TryGetValue(CacheConst.allCities, out allCityView))
            {
                return allCityView;
            }
            await semForGetAll.WaitAsync();
            try
            {
                if (cache.TryGetValue(CacheConst.allCities, out List<CityViewModel>? allCityV))
                {
                    return allCityV;
                }
                var allCityDB = await cityRepository.GetAllAsync();
                allCityView = mapper.Map<List<CityViewModel>>(allCityDB);
                cache.Set(CacheConst.allCities, allCityView, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
            }
            finally
            {
                semForGetAll.Release();
            }
            return allCityView;
        }

        public async Task<CityViewModel?> GetByIdAsync(int id)
        {
            if (cache.TryGetValue(new CityCacheKey(id), out CityViewModel? cityView))
            {
                return cityView;
            }
            await semForGetId.WaitAsync();
            try
            {
                if (cache.TryGetValue(new CityCacheKey(id), out cityView))
                {
                    return cityView;
                }
                var cityDB = await cityRepository.GetByIdAsync(id);
                cityView = mapper.Map<CityViewModel>(cityDB);
                if (cityView != null)
                {
                    cache.Set(new CityCacheKey(id), cityView);
                }
                else
                {
                    cache.Set(new CityCacheKey(id), cityView, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
                }

            }
            finally
            {
                semForGetId.Release();
            }
            return cityView;
        }

        public async Task<bool> Update(CityViewModel city)
        {
            var updateCityDb = mapper.Map<City>(city);
            var successUpdate = await cityRepository.Update(updateCityDb);
            if (successUpdate)
            {
                cache.Set(new CityCacheKey(city.Id.Value), city);
                return true;
            }
            return false;

        }
        public record CityCacheKey(int id);
    }
}
