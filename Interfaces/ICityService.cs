using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface ICityService
    {
        public Task<CityViewModel?> GetByIdAsync(int id);
        public Task<bool> AddAsync(CityViewModel city);
        public Task<bool> Update(CityViewModel city);
        public Task<bool> DeleteAsync(CityViewModel city);
        public Task<List<CityViewModel>?> GetAllAsync();

    }
}
