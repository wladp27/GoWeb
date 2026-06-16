using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Repositories;
using GoWebApplication.Db.Models;
using Microsoft.Extensions.Caching.Memory;
using static GoWeb.Service.CityService;
using static GoWeb.Service.UserService;

namespace GoWeb.Service
{
    public class RatingService
    {

        private readonly IRatingRepository ratingRepository;
        private readonly IMemoryCache cache;
        public RatingService(IRatingRepository ratingRepository, IMemoryCache cache) 
        {
            this.ratingRepository = ratingRepository;
            this.cache = cache;
        }
        //public async Task<bool> AddAsync(Rating rating)
        //{
        //    var resultAdd=await ratingRepository.AddAsync(rating);
        //    if (resultAdd != null)
        //    {
        //        cache.Remove(new UsersPreviewCacheKey())
        //        return true;
        //    }
        //    return false;

        //}

        //public async Task<Rating> GetByIdAsync(string idUser, int idTypeEvent)
        //{
           
        //}

        //public async Task<List<Rating>> GetByIdAsync(List<string> idUsers, int idTypeEvent)
        //{
            
        //}

        //public async Task<bool> UpdateAsync(Rating rating)
        //{
           
        //}
    }
}
