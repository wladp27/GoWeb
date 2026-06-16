using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class UserRegisterProfile : Profile
    {
        public UserRegisterProfile() 
        {
            CreateMap<User, UserRegisterViewModel>().ReverseMap();
        }
    }
}
