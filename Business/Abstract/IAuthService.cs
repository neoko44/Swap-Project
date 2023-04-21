using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<AccessToken> CreateAccessToken(User user);
        IDataResult<AccessToken> Register(RegisterDto registerDto);
        IDataResult<User> LoginMail(string email, string password);
        IDataResult<ChangePasswordDto> ChangePassword(ChangePasswordDto changePasswordDto);

        List<Operation> GetClaims(User user);
        IResult CheckEmail(string email);
        IResult CheckPhone(string phone);
        User GetByMail(string email);


    }
}
