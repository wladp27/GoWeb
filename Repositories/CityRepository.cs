using GoWeb.Interfaces;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoWeb.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> AddAsync(City city)
        {
            var existCityDb =  await ExistCity(city.NameCity);
            if(existCityDb)
            {
                return null;
            }
            await _context.Cities.AddAsync(city);
            var rowsAffected =  await _context.SaveChangesAsync();
            if (rowsAffected > 0)
                return city.Id;
            return null;
        }

        public async Task<bool> DeleteAsync(City city)
        {

            try 
            {
                int rowsAffected = await _context.Cities.Where(c => c.Id == city.Id).ExecuteDeleteAsync();
                if (rowsAffected == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false; 
            }
        }

        public async Task<List<City>> GetAllAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<City> GetByIdAsync(int id)
        {
           
            return await _context.Cities.FindAsync(id);
        }

        public async Task<bool> Update(City city)
        {

                _context.Cities.Update(city); // отслеживание в новом контексте включаем
                var rowsAffected = await _context.SaveChangesAsync();
                if (rowsAffected>0)
                    return true;
                return false;
           
        }

        public async Task<bool> ExistCity(string nameCity)
        {
           return  await _context.Cities.AnyAsync(c => c.NameCity == nameCity);
        }
    }
}