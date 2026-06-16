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

        public async Task<List<string?>> GetUsersNamesDB(int idEvent) // без контекста сделать
        {
          
            return await context.UsersEvents.Where(ue => ue.EventId == idEvent &&
                                                 (ue.StatusJoiningId == (int)JoiningStatus.Registered || ue.StatusJoiningId == (int)JoiningStatus.InReserve))
                                            .OrderBy(ue => ue.StatusJoiningId)
                                               .ThenBy(ue => ue.TimeJoinEvent)
                                            .Select(ue=>ue.User.UserName)
                                            .ToListAsync();
        }


        public async Task<List<UserPreviewView>> GetPreviewUsers(List<string>? nicknamesUsers)
        {
            var listUserPreview = new List<UserPreviewView>();
            var listUserNotInCache = new List<string>();
            foreach (var nickname in nicknamesUsers)
            {
                if (cache.TryGetValue(new UsersPreviewCacheKey(nickname), out UserPreviewView? userPreviewView))
                {
                    listUserPreview.Add(userPreviewView);
                }
                else
                {
                    listUserNotInCache.Add(nickname);
                }          
            }
            if (listUserNotInCache.Count > 0) // ограничение существует
            {
                var userNotInCache = await GetPreviewUsersDB(listUserNotInCache);
                listUserPreview.AddRange(userNotInCache);
                WriteUsersInCache(userNotInCache);
            }
            return listUserPreview;
        }

        public async Task<List<UserPreviewView>> GetPreviewUsersDB(List<string>? nicknamesUsers)
        {
            var usersPreview = await userRepository.GetAllUsersQueryable()
                                              .Where(e => nicknamesUsers.Contains(e.UserName))
                                              .ProjectTo<UserPreviewView>(mapper.ConfigurationProvider)
                                              .ToListAsync();
            return usersPreview;
        }

        public void WriteUsersInCache(List<UserPreviewView> usersPreview)
        {
            foreach (var userPrev in usersPreview)
            {
                cache.Set(new UsersPreviewCacheKey(userPrev.UserName), userPrev, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24)));
            }  
        }


        
        public record UsersPreviewCacheKey(string userName);
    }
}
