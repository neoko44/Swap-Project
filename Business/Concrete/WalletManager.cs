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
        ICryptoDal _cryptoDal;

        public WalletManager(IWalletDal walletDal, ITokenHelper tokenHelper, IUserDal userDal, ICryptoDal cryptoDal)
        {
            _walletDal = walletDal;
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _cryptoDal = cryptoDal;
        }

        public IDataResult<WalletDto> Deposit(Deposit deposit)
        {

            var tokenClaim = _tokenHelper.GetTokenInfo(); // kullanıcının tokeni decrypt ediliyor ve
                                                          // buradan sonra gerçek kişi olup olmadığı doğrulanacak
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<WalletDto>(Messages.UserNotFound);
            }

            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email); //tokeni getir. token içerisindeki maile
                                                                            //karşılık gelen kullanıcıyı veritabanından getir
            if (user == null)
            {
                return new ErrorDataResult<WalletDto>(Messages.UserNotFound);
            }

            if (deposit.Amount <= 0)
            {
                return new ErrorDataResult<WalletDto>(Messages.BalanceInvalid);
            }

            var getCrypto = _cryptoDal.Get(c => c.Id == deposit.CryptoId && c.Status == true);
            if (getCrypto == null)
            {
                return new ErrorDataResult<WalletDto>(Messages.CryptoNotFound);
            }

            if (decimal.TryParse(deposit.Amount.ToString(), out decimal tempMoney) && tempMoney >= 9999999999.99999999m)
            {
                return new ErrorDataResult<WalletDto>(Messages.BalanceInvalid);
            }

            var Wallet = _walletDal.Get(w => w.UserId == user.Id && w.Type == getCrypto.Name && w.Status == true);

            if (Wallet == null)
            {
                Wallet wallet = new()
                {
                    UserId = user.Id,
                    Type = getCrypto.Name,
                    Balance = deposit.Amount,
                    Status = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };
                _walletDal.Add(wallet);
            }

            else if (Wallet != null)
            {
                Wallet.Balance += deposit.Amount;
                Wallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(Wallet);
            }


            var getWallet = _walletDal.Get(w => w.UserId == user.Id && w.Type == getCrypto.Name && w.Status == true);
            WalletDto walletDto = new()
            {
                Id = getWallet.Id,
                Balance = getWallet.Balance,
                Symbol = getWallet.Type,
            };

            return new SuccessDataResult<WalletDto>(walletDto, Messages.MoneyAdded);
        }

        public IDataResult<List<Wallet>> GetListByUserId(int userId)
        {
            var result = _walletDal.GetList(w=>w.UserId == userId).ToList();
            if (result == null)
            {
                return new ErrorDataResult<List<Wallet>>(Messages.WalletNotFound);
            }
            return new SuccessDataResult<List<Wallet>>(result);

        }

        public IDataResult<List<WalletDto>> GetWallets()
        {
            var tokenClaim = _tokenHelper.GetTokenInfo(); // kullanıcının tokeni decrypt ediliyor ve
                                                          // buradan sonra gerçek kişi olup olmadığı doğrulanacak
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<List<WalletDto>>(Messages.UserNotFound);
            }

            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email); //tokeni getir. token içerisindeki maile
                                                                            //karşılık gelen kullanıcıyı veritabanından getir
            if (user == null)
            {
                return new ErrorDataResult<List<WalletDto>>(Messages.UserNotFound);
            }

            var getWallets = _walletDal.GetList(w=>w.UserId == user.Id).ToList();
            List<WalletDto> wallets = new();

            foreach ( var wallet in getWallets )
            {
                WalletDto walletDto = new()
                {
                    Id = wallet.Id,
                    Balance = wallet.Balance,
                    Symbol = wallet.Type,
                };
                wallets.Add(walletDto);
            }
            return new SuccessDataResult<List<WalletDto>>(wallets);
        }

        public void Update(Wallet wallet)
        {
            _walletDal.Update(wallet);
        }

        //public IDataResult<WalletDto> Withdraw(Deposit deposit)
        //{
        //    var tokenClaim = _tokenHelper.GetTokenInfo(); // kullanıcının tokeni decrypt ediliyor ve
        //                                                  // buradan sonra gerçek kişi olup olmadığı doğrulanacak
        //    if (tokenClaim.Data == null)
        //    {
        //        return new ErrorDataResult<WalletDto>(Messages.UserNotFound);
        //    }

        //    var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email); //tokeni getir. token içerisindeki maile
        //                                                                    //karşılık gelen kullanıcıyı veritabanından getir
        //    if (user == null)
        //    {
        //        return new ErrorDataResult<WalletDto>(Messages.UserNotFound);
        //    }
        //    var getCrypto = _cryptoDal.Get(c => c.Id == deposit.CryptoId && c.Status == true);
        //    if (getCrypto == null)
        //    {
        //        return new ErrorDataResult<WalletDto>(Messages.CryptoNotFound);
        //    }

        //    var getWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == getCrypto.Name);
        //    if (getWallet.Balance < deposit.Amount)
        //    {
        //        return new ErrorDataResult<WalletDto>(Messages.BalanceInvalid);
        //    }

        //    getWallet.Balance -= deposit.Amount;
        //    getWallet.UpdatedDate = DateTime.Now;
        //    _walletDal.Update(getWallet);

        //    var lastWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == );
        //    return new SuccessDataResult<WalletDto>();
        //}
    }
}
