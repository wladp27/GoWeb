using GoWeb.Interfaces;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Repositories
{
    public class RatingRepository :IRatingRepository
    {
        private readonly ApplicationDbContext context;
        public RatingRepository(ApplicationDbContext context) 
        {
            this.context = context;
        }

        public async Task<bool> AddAsync(Rating rating)
        {
            var existRating = await context.Ratings.FindAsync(new object[]{rating.UserName, rating.EventTypeId });
            if (existRating != null)
            {
                context.Ratings.Add(rating);
            }
            await context.Ratings.AddAsync(rating);
            var rowsAffected = await context.SaveChangesAsync();
            if (rowsAffected > 0)
                return true;
            return false;

        }

        public async Task<Rating> GetByIdAsync(string idUser,int idTypeEvent)
        {
           return await context.Ratings.FindAsync(new object[] { idUser, idTypeEvent });
        }

        public async Task<List<Rating>> GetByIdAsync(List<string> userNames, int idTypeEvent)
        {
           return  await context.Ratings.Where(r=> userNames.Contains(r.UserName) && idTypeEvent==r.EventTypeId)
                                 .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Rating rating)
        {
            context.Ratings.Update(rating); // отслеживание в новом контексте включаем
            var rowsAffected = await context.SaveChangesAsync();
            if (rowsAffected > 0)
                return true;
            return false;
        }
    }
}
