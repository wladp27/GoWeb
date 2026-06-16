using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface ICityRepository
    {
        public Task<City> GetByIdAsync(int id);
        public Task<int?> AddAsync(City city);
        public Task<bool> Update(City city);
        public Task<bool> DeleteAsync(City city);
        public Task<List<City>> GetAllAsync();

        public Task<bool> ExistCity(string nameCity);

    }
}
