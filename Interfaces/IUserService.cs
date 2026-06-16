using AutoMapper;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Сonstants;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Interfaces
{
    public interface IUserService
    {




        public  Task<List<string?>> GetUsersNamesDB(int idEvent);
        public  Task<List<UserPreviewView>> GetPreviewUsers(List<string>? nicknamesUsers);


        public Task<List<UserPreviewView>> GetPreviewUsersDB(List<string>? nicknamesUsers);


        public void WriteUsersInCache(List<UserPreviewView> usersPreview);


     
    }
}
