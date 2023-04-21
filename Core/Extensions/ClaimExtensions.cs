using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }

        public static void AddName(this ICollection<Claim> claims, string name)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, name));
        }

        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, nameIdentifier));
        }

        public static void AddRoles(this ICollection<Claim> claims, string[] roles)
        {
            
            roles.ToList().ForEach(role => claims.Add(new Claim("role", role.TrimEnd())));
        }

        public static void AddPhone(this ICollection<Claim> claims, string phone)
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, phone));
        }

        public static void AddCity(this ICollection<Claim> claims, string city)
        {
            claims.Add(new Claim(ClaimTypes.Country, city));    
        }
    }
}
