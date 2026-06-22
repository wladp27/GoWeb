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

        public async Task<bool> AddAsync(string idUser,int idEventType, int value) 
        {
            if (value < 0 || value > 100) return false; 
            try
            {
                var rating = new Rating() { UserId = idUser, EventTypeId = idEventType, Value = value };
                context.Ratings.Add(rating);
                var rowsAffected = await context.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateException)
            {
                return false;
            }
            return false;
        }

        public async Task<Rating> GetByIdAsync(string idUser,int idTypeEvent)
        {
           return await context.Ratings.FindAsync(new object[] { idUser, idTypeEvent });
        }

    

        public async Task<bool> UpdateAsync(Rating rating)
        {
            if (rating.Value < 0 || rating.Value > 100) return false;
            context.Ratings.Update(rating); 
            var rowsAffected = await context.SaveChangesAsync();
            if (rowsAffected > 0)
                return true;
            return false;
        }
    }
}
