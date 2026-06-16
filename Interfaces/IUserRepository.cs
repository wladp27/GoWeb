using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;

namespace GoWeb.Interfaces
{
    public interface IUserRepository
    {
        public  Task<IdentityResult> CreateAsync(User user,string password);
        public  Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        public Task<bool> CheckPasswordAsync(User user, string password);
        public  Task<User> FindByIdAsync(string id);
        public  Task<User> FindByEmailAsync(string email);
        public  Task<User> FindByNameAsync(string name);
        public  Task<IdentityResult> UpdateAsync(User user);
        public  Task<List<User>> Users();
        public  Task<IdentityResult> AddToRoleAsync(User user, string role);
        public  Task<IList<string>> GetRolesAsync(User user);
        public  Task<bool> IsInRoleAsync(User user, string roleName);
        public  Task<IdentityResult> RemoveFromRoleAsync(User user, string roleName);
        public Task<int?> GetByIdWithCityAsync(string userId);

        public Task<bool> ExistenceUser(string userId);
        public IQueryable<User> GetAllUsersQueryable();
        Task<List<(string UserName, string Id)>> GetUsers(string userName);
    }
}
