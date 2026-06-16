using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;

namespace GoWeb.Mapping
{
    public class CityProfile : Profile
    {
        public CityProfile() 
        {
            CreateMap<City,CityViewModel>().ReverseMap();
            
        }
    }
}
