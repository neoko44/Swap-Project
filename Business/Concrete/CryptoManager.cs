using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CryptoManager : ICryptoService
    {
        IParityService _parityService;
        ITokenHelper _tokenHelper;
        IUserDal _userDal;
        IWalletDal _walletDal;
        ICompanyWalletDal _companyWalletDal;
        ICryptoDal _cryptoDal;
        IOrderService _orderService;
        ISellOrderDal _sellOrderDal;


        public CryptoManager(IParityService parityService, ITokenHelper tokenHelper, IUserDal userDal, IWalletDal walletDal, ICompanyWalletDal commissionDal, ICryptoDal cryptoDal, IOrderService orderService, ISellOrderDal sellOrderDal)
        {
            _parityService = parityService;
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _walletDal = walletDal;
            _companyWalletDal = commissionDal;
            _cryptoDal = cryptoDal;
            _orderService = orderService;
            _sellOrderDal = sellOrderDal;
        }

        public async Task<IResult> BuyCrypto(BuyCryptoDto buyCryptoDto)
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

            var wallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true);//kullanıcının cüzdanını getir
            if (wallet == null)
            {
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }

            var crypto = _cryptoDal.Get(c => c.Id == buyCryptoDto.Id && c.Status == true);//SQL'den kriptoyu getir
            if (crypto == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CryptoNotFound);
            }

            var parity = _parityService.GetPrice(buyCryptoDto.Id).Result;//api'ye istek at ve anlık fiyatı getir.
            if (!parity.Success)
            {
                return new ErrorDataResult<Crypto>(Messages.ParityNotFound);
            }

            var cultureInfo = CultureInfo.InvariantCulture;

            var SaltPrice = buyCryptoDto.Price - buyCryptoDto.Price * crypto.Commission;//tutardan komisyonu çıkart
            var SaltAmount = buyCryptoDto.Amount - buyCryptoDto.Amount * crypto.Commission;//miktardan komisyonu çıkart

            if (wallet.Balance < buyCryptoDto.Price * buyCryptoDto.Amount)
            {
                return new ErrorDataResult<Wallet>(Messages.NotEnoughBalance);
            }//bakiye (kullanıcının işlem tutarı + komisyon) değerinden düşük ise

            var checkValue = CheckValue(decimal.Parse(parity.Data.price,cultureInfo), buyCryptoDto.Price);//girilen değer min-max aralıkta mı kontrol et
            if (!checkValue.Success)
            {
                return new ErrorDataResult<Parity>(checkValue.Message);
            }

            decimal collected = 0;

            var getSellOrders = _sellOrderDal.GetList(s=>s.Status == Types.OrderStatus.Waiting.ToString()).OrderBy(s=>s.CreatedDate).ToList();
            if (getSellOrders.Count > 0)
            {
                foreach (var Sell in getSellOrders)
                {

                }



            }
            
            BuyOrder buyOrder = new()
            {
                UserId = user.Id,
                CryptoId = crypto.Id,
                Parity = parity.Data.symbol,
                ParityPrice = decimal.Parse(parity.Data.price, cultureInfo),
                Price = buyCryptoDto.Price,
                Amount = SaltAmount,
                Total = buyCryptoDto.Price * buyCryptoDto.Amount,
                Type = Types.OrderTypes.Buy.ToString(),
                CommissionFee = crypto.Commission * buyCryptoDto.Amount,
                Collected = collected,
                Status = Types.OrderStatus.Waiting.ToString(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            _orderService.CreateBuyOrder(buyOrder);

            wallet.Balance -= SaltPrice;
            wallet.UpdatedDate = DateTime.Now;
            _walletDal.Update(wallet);



            var companyWallet = _companyWalletDal.Get(c => c.CryptoId == buyCryptoDto.Id);//SQL'den şirket cüzdanını getir

            if (companyWallet != null)
            {
                companyWallet.Total += decimal.Parse(parity.Data.price) * crypto.Commission * buyCryptoDto.Amount;
                _companyWalletDal.Update(companyWallet);
            }//komisyon var ise total değeri güncelle

            else if (companyWallet == null)
            {
                CompanyWallet commission = new()
                {
                    CryptoId = buyCryptoDto.Id,
                    Total = decimal.Parse(parity.Data.price, cultureInfo) * crypto.Commission * buyCryptoDto.Amount,
                };
                _companyWalletDal.Add(commission);
            }//komisyon yok ise yeni komisyon oluştur ve komisyon tutarını ver

            return new SuccessResult();


        }

        public IResult CheckValue(decimal parityPrice, decimal userPrice)
        {
            //int mx = 5; //min-max çarpan değeri
            //int v = 100; //aralık değeri

            if (parityPrice > 0 && parityPrice < 100) //0 ile 100 arasında ise
            {
                if (userPrice < parityPrice - 5 || userPrice > parityPrice + 5)
                {
                    return new ErrorDataResult<Parity>(Messages.InvalidValue);
                }
            }

            for (int mx = 20, v = 100; v < parityPrice; v *= 10, mx *= 4)
            {
                if (parityPrice > v && parityPrice < v * 10) //100 ile 1000 arasında ise
                {
                    if (userPrice < parityPrice - mx || userPrice > parityPrice + mx) //değer min-max arasında değil ise
                    {
                        return new ErrorDataResult<Parity>(Messages.InvalidValue); //hata döndür
                    }
                }
            }

            return new SuccessDataResult<Parity>(Messages.ValidValue);
        }

    }
}
