using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        IUserDal _userDal;
        ITokenHelper _tokenHelper;
        IUserOperationDal _operationDal;
        IWalletDal _walletDal;

        public AuthManager(IUserDal userDal, ITokenHelper tokenHelper, IUserOperationDal operationDal, IWalletDal walletDal)
        {
            _userDal = userDal;
            _tokenHelper = tokenHelper;
            _operationDal = operationDal;
            _walletDal = walletDal;
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userDal.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }

        public IDataResult<AccessToken> Register(RegisterDto registerDto)
        {

            if (registerDto.Password == null || registerDto.Password == "")
            {
                return new ErrorDataResult<AccessToken>(Messages.PasswordNull);
            }
            if (registerDto.Email == null || registerDto.Email == "")
            {
                return new ErrorDataResult<AccessToken>(Messages.EmailNull);
            }
            if (registerDto.FirstName == null || registerDto.FirstName == "")
            {
                return new ErrorDataResult<AccessToken>(Messages.FirstNameNull);
            }
            if (registerDto.LastName == null || registerDto.LastName == "")
            {
                return new ErrorDataResult<AccessToken>(Messages.LastNameNull);
            }
            if (registerDto.Phone == null || registerDto.Phone == "")
            {
                return new ErrorDataResult<AccessToken>(Messages.PhoneNull);
            }
            if (!CheckEmail(registerDto.Email).Success)
            {
                return new ErrorDataResult<AccessToken>(Messages.MailExists);
            }
            if (!CheckPhone(registerDto.Phone).Success)
            {
                return new ErrorDataResult<AccessToken>(Messages.PhoneExists);
            }



            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(registerDto.Password, out passwordHash, out passwordSalt);

            User user = new()
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                RoleId = 1,
                Status = true,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };
            _userDal.Add(user);

            UserOperation userOperation = new()
            {
                OperationId = 1,
                UserId = user.Id,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };
            _operationDal.Add(userOperation);

            Wallet wallet = new()
            {
                UserId = user.Id,
                Balance = 0,
                Type = Types.Parities.USDT.ToString().ToUpper(),
                Status = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };
            _walletDal.Add(wallet);

            var accessToken = CreateAccessToken(user);
            if (accessToken == null)
            {
                return new ErrorDataResult<AccessToken>(Messages.AccessTokenError);
            }

            return new SuccessDataResult<AccessToken>(accessToken.Data, Messages.UserRegistered.ToString());
        }

        public IDataResult<User> LoginMail(string email, string password)
        {
            var userToCheck = GetByMail(email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }

            if (password == null)
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }

            if (!HashingHelper.VerifyPasswordHash(password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            userToCheck.UpdatedDate = DateTime.Now;
            _userDal.Update(userToCheck);
            return new SuccessDataResult<User>(userToCheck, Messages.SuccessfulLogin);
        }

        public IDataResult<ChangePasswordDto> ChangePassword(ChangePasswordDto changePasswordDto)
        //giriş yapan kullanıcının şifresini değiştir
        {
            var tokenClaim = _tokenHelper.GetTokenInfo(); // kullanıcının tokeni decrypt ediliyor ve
                                                          // buradan sonra gerçek kişi olup olmadığı doğrulanacak
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }

            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email); //tokeni getir. token içerisindeki maile
                                                                            //karşılık gelen kullanıcıyı veritabanından getir
            if (user == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.PasswordNotMatch);
            }

            if (HashingHelper.VerifyPasswordHash(changePasswordDto.NewPassword, user.PasswordHash, user.PasswordSalt))
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.NewPassMustDifferent);
            }
            else
            {
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(changePasswordDto.NewPassword, out passwordHash, out passwordSalt);//eski şifreyi kontrol et
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                user.UpdatedDate = DateTime.Now;

                _userDal.Update(user);
            }
            return new SuccessDataResult<ChangePasswordDto>(Messages.PassChangeSuccess);
        }

        public List<Operation> GetClaims(User user)
        {
            return _userDal.GetClaims(user);
        }
        public IResult CheckEmail(string email)
        {
            var getMail = _userDal.Get(u => u.Email == email);
            if (getMail != null)
            {
                return new ErrorResult(Messages.MailExists);
            }
            return new SuccessResult();
        }
        public IResult CheckPhone(string phone)
        {
            var getUser = _userDal.Get(u => u.Phone == phone);
            if (getUser != null)
            {
                return new ErrorResult(Messages.PhoneExists);
            }
            return new SuccessResult();
        }
        public User GetByMail(string email)
        {
            return _userDal.Get(u => u.Email == email);
        }
    }
}

