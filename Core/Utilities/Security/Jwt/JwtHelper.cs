using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Encryption;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        TokenOptions _tokenOptions;
        DateTime _accessTokenExpiration;
        IHttpContextAccessor _contextAccessor;
        public JwtHelper(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _contextAccessor = contextAccessor;
        }

        public AccessToken CreateToken(User user, List<Operation> operationClaims)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };

        }
        public string GetTokenInfo(string idtoken)
        {
            var token = new JwtSecurityToken(jwtEncodedString: idtoken);
            string email = token.Claims.First(c => c.Type == "email").Value;
            string phone = token.Claims.First(c => c.Type == "mobilephone").Value;
            string NameSurname = token.Claims.First(c => c.Type == "name").Value;
            string UserId = token.Claims.First(c => c.Type == "nameidentifier").Value;
            return new List<string> { email, phone, NameSurname }.ToString();
        }
        //public string DecryptJwtToken(string token)
        //{

        //}
        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, List<Operation> operationClaims)
        {
            var jwt = new JwtSecurityToken
                (
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
                );
            return jwt;
        }
        private IEnumerable<Claim> SetClaims(User user, List<Operation> operationClaims)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(user.Id.ToString());
            claims.AddEmail(user.Email);
            claims.AddName($"{user.FirstName} {user.LastName}");
            claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
            return claims;
        }

        public IDataResult<TokenInfoDto> GetTokenInfo()
        {
            string token = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").LastOrDefault();

            if (token == null)
            {
                return new ErrorDataResult<TokenInfoDto>("Kullanıcı bulunamadı");
            }
            var DecryptedToken = new JwtSecurityToken(jwtEncodedString: token);

            string Email = DecryptedToken.Claims.First(c => c.Type == "email").Value;
            string NameSurname = DecryptedToken.Claims.First(c => c.Type == "name").Value;
            string UserId = DecryptedToken.Claims.First(c => c.Type == "nameid").Value;
            string Role = DecryptedToken.Claims.First(c => c.Type == "role").Value;

            TokenInfoDto tokenInfoDto = new()
            {
                Email = Email,
                NameSurname = NameSurname,
                Id = UserId,
                Role = Role
            };

            return new SuccessDataResult<TokenInfoDto>(tokenInfoDto);
        }
    }
}
