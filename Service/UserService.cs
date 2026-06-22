using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GoWeb.Service
{
    public class UserService:IUserService
    {
        private readonly IMemoryCache cache;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        public UserService(IMemoryCache cache, IMapper mapper, IUserRepository userRepository, ApplicationDbContext context) 
        {
            this.cache = cache;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.context = context;
        }

        public async Task<List<string>> GetIdUsersDB(int idEvent) // без контекста сделать
        {
          
            return await context.UsersEvents.Where(ue => ue.EventId == idEvent &&
                                                 (ue.StatusJoiningId == (int)JoiningStatus.Registered || ue.StatusJoiningId == (int)JoiningStatus.InReserve))
                                            .OrderBy(ue => ue.StatusJoiningId)
                                               .ThenBy(ue => ue.TimeJoinEvent)
                                            .Select(ue=>ue.User.Id)
                                            .ToListAsync();
        }


        public async Task<List<UserPreviewView>> GetPreviewUsers(List<string>? idUsers)
        {
            var listUserPreview = new List<UserPreviewView>();
            var idUserNotInCache = new List<string>();
            foreach (var id in idUsers)
            {
                if (cache.TryGetValue(new UsersPreviewCacheKey(id), out UserPreviewView? userPreviewView))
                {
                    listUserPreview.Add(userPreviewView);
                }
                else
                {
                    idUserNotInCache.Add(id);
                }          
            }
            if (idUserNotInCache.Count > 0) // ограничение существует
            {
                var userNotInCache = await GetPreviewUsersDB(idUserNotInCache);
                listUserPreview.AddRange(userNotInCache);
                WriteUsersInCache(userNotInCache);
            }
            return listUserPreview;
        }

        public async Task<List<UserPreviewView>> GetPreviewUsersDB(List<string>? idUsers)
        {
            var usersPreview = await userRepository.GetAllUsersQueryable()
                                              .Where(e => idUsers.Contains(e.Id))
                                              .ProjectTo<UserPreviewView>(mapper.ConfigurationProvider)
                                              .ToListAsync();
            return usersPreview;
        }


        public async Task<UserPreviewView> GetPreviewUserDB(string idUsers)
        {
            var usersPreview = await userRepository.GetAllUsersQueryable()
                                .ProjectTo<UserPreviewView>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(e => e.Id == idUsers);
            return usersPreview;
        }



        public void WriteUsersInCache(List<UserPreviewView> usersPreview)
        {
            foreach (var userPrev in usersPreview)
            {
                cache.Set(new UsersPreviewCacheKey(userPrev.Id), userPrev, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24)));
            }  
        }


        public  async Task<UserPreviewView> GetPreviewUser(string idUser)
        {
            if (cache.TryGetValue(new UsersPreviewCacheKey(idUser), out UserPreviewView? userPreviewView))
            {
                return userPreviewView;
            }

            var userPrevDb = await GetPreviewUserDB(idUser);
            if (userPrevDb != null)
            {
                cache.Set(new UsersPreviewCacheKey(userPrevDb.Id), userPrevDb, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24)));
            }
            return userPrevDb;
        }


        public record UsersPreviewCacheKey(string userId);
    }
}
