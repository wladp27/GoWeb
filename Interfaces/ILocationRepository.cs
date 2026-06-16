using GoWeb.Сonstants;
using GoWebApplication.Db.Models;

namespace GoWeb.Interfaces
{
    public interface ILocationRepository
    {
        public Task<Location> GetByIdAsync(int id);
        public Task AddAsync(Location location);
        public Task Update(Location location);
        public Task DeleteAsync(Location location);
        public Task<List<Location>> GetAllAsync();
        public Task<bool> ExistsAddress(string address, int idCity);
        public Task<List<(string addrres, int idLocation)>> GetLocations(string address,int idCity);
        public Task<bool> ExistsAddress(int idLocation);
        public Task<VerificationAddressResult> VerificationAddress(Location location);
    }
}
