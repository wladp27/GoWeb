using AutoMapper;
using GoWeb.Models;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;

namespace GoWeb.Mapping
{
    public class RoleProfile :Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleView>().ReverseMap();
        }

    }
}
