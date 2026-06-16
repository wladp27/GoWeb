using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface IRatingRepository
    {
        public Task<Rating> GetByIdAsync(string idUser, int idTypeEvent);
        public Task<List<Rating>> GetByIdAsync(List<string> userNames, int idTypeEvent);
        public Task<bool> AddAsync(Rating rating);
        public Task<bool> UpdateAsync(Rating rating);

    }
}
