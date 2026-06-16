using GoWeb.Interfaces;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext context;
        public LocationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(Location location)
        {
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Location location)
        {
            int rowsAffected = await context.Locations.Where(c => c.Id == location.Id).ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                // выводить логи
            }
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await context.Locations.AsNoTracking().ToListAsync();
        }

        public async Task<Location> GetByIdAsync(int id)
        {
            return await context.Locations.FindAsync(id);
        }

        public async Task Update(Location location)
        {
            context.Locations.Update(location); // отслеживание в новом контексте включаем
            await context.SaveChangesAsync();
        }

        public async Task<List<(string addrres, int idLocation)>> GetLocations(string address, int idCity)
        {
            return await context.Locations.Where(l=>l.Address.Contains(address) && idCity==l.CityId)
                             .Take(5)
                             .Select(l=>ValueTuple.Create(l.Address,l.Id))
                             .ToListAsync();
        }

        public async Task<bool> ExistsAddress(string address, int cityId)
        {
            return await context.Locations.AnyAsync(l => l.Address == address && l.CityId== cityId);
        }
        public async Task<bool> ExistsAddress(int idLocation)
        {
            return await context.Locations.AnyAsync(l => l.Id == idLocation);
        }
        public async Task<VerificationAddressResult> VerificationAddress(Location location)
        {
            var city = await context.Cities.AsNoTracking().FirstOrDefaultAsync(c=>c.Id== location.CityId);
            if (city == null)
            {
                return VerificationAddressResult.NonExistentCity;
            }
            else 
            {
               var exists =  await ExistsAddress(location.Address,location.CityId);
                if (!exists)
                {
                   var distanceFromCenter = CalculateDistance(location.LocationLatitude,location.LocationLongitude,city.LocationLatitude,city.LocationLongitude);
                    if (distanceFromCenter > 30)
                    {
                        return VerificationAddressResult.LocationOutsideCity;
                    }
                    else
                    {
                        return VerificationAddressResult.Ok;
                    }
                }
                else
                {
                    return VerificationAddressResult.NonExistentLocation;
                }

            }
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);


            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return 6371.0 * c; 
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
