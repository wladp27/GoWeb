using AutoMapper;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Сonstants;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Interfaces
{
    public interface IUserService
    {

        public  Task<List<string?>> GetIdUsersDB(int idEvent);
        public  Task<List<UserPreviewView>> GetPreviewUsers(List<string>? idUsers);
        public Task<UserPreviewView> GetPreviewUser(string idUser);
        public Task<List<UserPreviewView>> GetPreviewUsersDB(List<string>? idUsers);
        public void WriteUsersInCache(List<UserPreviewView> usersPreview);


     
    }
}
