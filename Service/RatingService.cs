using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Repositories;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Caching.Memory;
using static GoWeb.Service.CityService;
using static GoWeb.Service.UserService;

namespace GoWeb.Service
{
    public class RatingService: IRatingService
    {

        private readonly IRatingRepository ratingRepository;
        private readonly IMemoryCache cache;
        public RatingService(IRatingRepository ratingRepository, IMemoryCache cache) 
        {
            this.ratingRepository = ratingRepository;
            this.cache = cache;
        }

        public async Task<bool> AddAsync(string idUser, int idEventType, int value)
        {
            var resultAdd = await ratingRepository.AddAsync(idUser, idEventType, value);
            if (resultAdd)
            {
                cache.Remove(new UsersPreviewCacheKey(idUser));
                return true;
            }
            return false;

        }

        public async Task<Rating> GetByIdAsync(string idUser, int idTypeEvent)
        {
           return await ratingRepository.GetByIdAsync(idUser,idTypeEvent);
        }



        public async Task<bool> UpdateAsync(Rating rating)
        {
           var resultUpdate = await ratingRepository.UpdateAsync(rating);
            if(resultUpdate)
            {
                cache.Remove(new UsersPreviewCacheKey(rating.UserId));
                return true;
            }
            return false;
        }
    }
}
