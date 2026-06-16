using GoWeb.Сonstants.Claims;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
{
    public MyUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    // Этот метод вызывается каждый раз, когда Identity «собирает» пользователя
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaims(ClaimsConst.GetClaims(user)); 
        return identity;
    }
}