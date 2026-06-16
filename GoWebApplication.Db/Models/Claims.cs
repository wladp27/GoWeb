using GoWebApplication.Db.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace GoWeb.Сonstants.Claims
{
    public static class ClaimsConst
    {
        public static List<string> listClaims = new List<string>
        {
            "idCity"
        };
        public static List<Claim> GetClaims(User user)
        {
            var authClaims= new List<Claim>();
            foreach (var claim in listClaims)
            {
                var property = user.GetType().GetProperty(claim);
                authClaims.Add(new Claim(claim, property.GetValue(user).ToString()));
            }
            return authClaims;
        }
    }
}