using GoWeb.Interfaces;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace GoWeb.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;

        public UserRepository(UserManager<User> userManager) 
        {
            this.userManager = userManager;

        }
        public async Task<IdentityResult> CreateAsync(User user,string password)
        {
          user.RegistrationDate = DateTime.Now;
          return await userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword,string newPassword)
        {
            return await userManager.ChangePasswordAsync(user,oldPassword,newPassword);
        }

        //public async Task DeleteAsync(user)()
        //{
        //   await userManager.
        //}

        public async Task<User> FindByIdAsync(string id)
        {
           return await userManager.FindByIdAsync(id);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
           return await userManager.FindByEmailAsync(email);
        }

        public async Task<User> FindByNameAsync(string name)
        {
            return  await userManager.FindByNameAsync(name);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await userManager.UpdateAsync(user);
        }

        public async Task<List<User>> Users()
        {
            
            return await userManager.Users.ToListAsync();
        }

        public async Task<IdentityResult> AddToRoleAsync (User user,string role)
        {
            return await userManager.AddToRoleAsync(user, role);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user,string roleName)
        {
            return await userManager.IsInRoleAsync(user,roleName);
        }
        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string roleName)
        {
            return await userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<int?> GetByIdWithCityAsync(string userId)
        {
            return await userManager.Users
                                          .Where(u=>u.Id==userId)
                                          .Select(u=>u.idCity).FirstOrDefaultAsync();


        }

        public async Task<List<(string UserName,string Id)>> GetUsers(string userName)
        {
            return await userManager.Users
                              .Where(u => u.UserName.Contains(userName))
                              .Take(5)
                              .Select(u => ValueTuple.Create(u.UserName, u.Id))
                              .ToListAsync();
            

        }


        public IQueryable<User> GetAllUsersQueryable()
        {
            return userManager.Users.AsQueryable();
        }

        public async Task<bool> ExistenceUser(string userId)
        {
            return await userManager.Users.AllAsync(u => u.Id == userId);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
    }
}
