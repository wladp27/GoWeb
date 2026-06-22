using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IRatingRepository
    {
        public Task<Rating> GetByIdAsync(string idUser, int idTypeEvent);
        public Task<bool> AddAsync(string idUser,int idEventType,int value);
        public Task<bool> UpdateAsync(Rating rating);

    }
}
