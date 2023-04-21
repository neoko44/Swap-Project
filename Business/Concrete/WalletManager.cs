using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class WalletManager : IWalletService
    {
        IWalletDal _walletDal;
        ITokenHelper _tokenHelper;
        IUserDal _userDal;

        public WalletManager(IWalletDal walletDal, ITokenHelper tokenHelper, IUserDal userDal)
        {
            _walletDal = walletDal;
            _tokenHelper = tokenHelper;
            _userDal = userDal;
        }

        public IResult Deposit(decimal money)
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

            if (money <= 0)
            {
                return new ErrorDataResult<Wallet>(Messages.BalanceInvalid);
            }

            if (decimal.TryParse(money.ToString(), out decimal tempMoney) && tempMoney >= 9999999999.99999999m)
            {
                return new ErrorDataResult<Wallet>(Messages.BalanceInvalid);
            }

            var Wallet = _walletDal.Get(w => w.UserId == user.Id);

            Wallet.Balance += money;
            Wallet.UpdatedDate = DateTime.Now;
            _walletDal.Update(Wallet);

            var getWallet = _walletDal.Get(w => w.UserId == user.Id);

            return new SuccessDataResult<decimal>(getWallet.Balance, Messages.MoneyAdded);
        }

        public IResult Withdraw(decimal money)
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

            var getWallet = _walletDal.Get(w => w.UserId == user.Id);
            if (getWallet.Balance < money)
            {
                return new ErrorDataResult<string>($"Bakiyeniz: {getWallet.Balance}\n Çekmek istediğiniz tutar: {money}", Messages.BalanceInvalid);
            }

            getWallet.Balance -= money;
            getWallet.UpdatedDate = DateTime.Now;
            _walletDal.Update(getWallet);
            return new SuccessDataResult<string>($"Çekilen tutar: {money}\nKalan bakiye: {getWallet.Balance}", Messages.BalanceUpdated);
        }
    }
}
