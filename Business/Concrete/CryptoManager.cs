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
        ICryptoDal _cryptoDal;
        ITokenHelper _tokenHelper;
        IParityService _parityService;
        IOrderService _orderService;

        IUserDal _userDal;
        IWalletDal _walletDal;
        ICompanyWalletDal _companyWalletDal;

        public CryptoManager(IParityService parityService, ITokenHelper tokenHelper, IOrderService orderService, IUserDal userDal, IWalletDal walletDal, ICompanyWalletDal companyWalletDal, ICryptoDal cryptoDal)
        {
            _parityService = parityService;
            _tokenHelper = tokenHelper;
            _orderService = orderService;
            _userDal = userDal;
            _walletDal = walletDal;
            _companyWalletDal = companyWalletDal;
            _cryptoDal = cryptoDal;

        }


        public async Task<IResult> BuyCrypto(BuyCryptoDto buyCryptoDto)
        {
            var tokenClaim = _tokenHelper.GetTokenInfo();
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }
            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email && x.Status == true);
            if (user == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }
            var parity = await _parityService.GetPrice(buyCryptoDto.Id);//api'ye istek at ve anlık fiyatı getir.
            var cultureInfo = CultureInfo.InvariantCulture;
            var checkValue = CheckValue(decimal.Parse(parity.Data.price, cultureInfo), buyCryptoDto.Price);//girilen değer min-max aralıkta mı kontrol et
            var crypto = _cryptoDal.Get(c => c.Id == buyCryptoDto.Id && c.Status == true);//SQL'den kriptoyu getir
            var cryptoUSDT = _cryptoDal.Get(c => c.Name == Types.Parities.USDT.ToString() && c.Status == true);
            var cryptoWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == crypto.Name);
            var UsdtWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == Types.Parities.USDT.ToString());//kullanıcının cüzdanını getir
            var companyUSDTWallet = _companyWalletDal.Get(cw => cw.CryptoId == cryptoUSDT.Id);//SQL'den şirket cüzdanını getir
            var companyCryptoWallet = _companyWalletDal.Get(cw => cw.CryptoId == crypto.Id);
            decimal remaining = buyCryptoDto.Amount;
            decimal commissionFee = 0;

            if (buyCryptoDto.Amount <= 0)
            {
                return new ErrorDataResult<Crypto>(Messages.InvalidAmount);
            }
            if (buyCryptoDto.Price <= 0)
            {
                return new ErrorDataResult<Crypto>(Messages.InvalidPrice);
            }
            if (!parity.Success)
            {
                return new ErrorDataResult<Crypto>(Messages.ParityNotFound.ToString());
            }
            if (checkValue.Success == false)
            {
                return new ErrorDataResult<Parity>(checkValue.Message);
            }
            if (crypto == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CryptoNotFound);
            }
            if (cryptoUSDT == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CompanyUsdtWalletNotFound);
            }
            if (cryptoWallet == null)
            {
                Wallet wallet1 = new()
                {
                    UserId = user.Id,
                    Type = crypto.Name,
                    Balance = 0,
                    Status = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };
                cryptoWallet = wallet1;
                _walletDal.Add(cryptoWallet);
            }
            if (UsdtWallet == null)
            {
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }
            if (UsdtWallet.Balance < buyCryptoDto.Price * buyCryptoDto.Amount)
            {
                return new ErrorDataResult<Wallet>(Messages.NotEnoughBalance);
            }//bakiye (kullanıcının işlem tutarı + komisyon) değerinden düşük ise
            if (companyUSDTWallet == null)
            {
                CompanyWallet companyWallet1 = new()
                {
                    CryptoId = cryptoUSDT.Id,
                    Total = 0
                };
                companyUSDTWallet = companyWallet1;
                _companyWalletDal.Add(companyUSDTWallet);
            }

            BuyOrder buyOrder = new()
            {
                UserId = user.Id,
                CryptoId = crypto.Id,
                Parity = parity.Data.symbol,
                ParityPrice = decimal.Parse(parity.Data.price, cultureInfo),
                Price = buyCryptoDto.Price,
                Amount = buyCryptoDto.Amount,
                Total = buyCryptoDto.Price * buyCryptoDto.Amount,
                CommissionFee = crypto.Commission * commissionFee,
                Remaining = remaining,
                Collected = 0,
                Status = Types.OrderStatus.Waiting.ToString(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };


            var getSellOrders = _orderService.GetSellOrders().Data.FindAll(s => s.Status == Types.OrderStatus.Waiting.ToString() && s.Price == buyCryptoDto.Price && s.UserId != user.Id).OrderBy(s => s.CreatedDate).ToList();
            if (getSellOrders.Count > 0)
            {
                foreach (var Sell in getSellOrders) 
                {
                    if (Sell.Remaining < buyOrder.Remaining)//eğer satıştaki kripto alıştaki kriptodan düşük ise
                    {
                        buyOrder.Collected += Sell.Remaining;
                        buyOrder.Remaining = buyOrder.Amount - buyOrder.Collected;//bu değeri alım emrindeki toplanan kriptolara ekle
                        buyOrder.UpdatedDate = DateTime.Now; // tarihini güncelle

                        Sell.Collected = Sell.Amount;
                        Sell.Status = Types.OrderStatus.Completed.ToString();
                        Sell.Remaining = 0;
                        Sell.UpdatedDate = DateTime.Now;
                        Sell.CommissionFee = crypto.Commission * Sell.Collected;
                        _orderService.UpdateSellOrder(Sell);

                        var SellUsdtWallet = _walletDal.Get(w => w.Status == true && w.UserId == Sell.UserId && w.Type == Types.Parities.USDT.ToString());

                        SellUsdtWallet.Balance += Sell.Total - Sell.Price * Sell.CommissionFee;
                        SellUsdtWallet.UpdatedDate = DateTime.Now;
                        _walletDal.Update(SellUsdtWallet);

                        companyCryptoWallet.Total += Sell.CommissionFee;
                        _companyWalletDal.Update(companyCryptoWallet);

                    }

                    if (buyOrder.Remaining < Sell.Remaining) //buyorder düşük ise buyorderi kapat ve komisyonu al kalan bakiyeyi cüzdana ekle
                    {
                        buyOrder.Status = Types.OrderStatus.Completed.ToString();
                        buyOrder.Collected = buyOrder.Amount;
                        buyOrder.Remaining = 0;
                        buyOrder.CommissionFee = buyOrder.Amount * crypto.Commission;
                        buyOrder.UpdatedDate = DateTime.Now;

                        Sell.Collected += buyOrder.Collected;
                        Sell.Remaining = Sell.Amount - Sell.Collected;
                        Sell.CommissionFee = Sell.Collected * crypto.Commission;
                        Sell.UpdatedDate = DateTime.Now;
                        _orderService.UpdateSellOrder(Sell);


                        companyUSDTWallet.Total += buyOrder.CommissionFee * buyOrder.Price;
                        _companyWalletDal.Update(companyUSDTWallet);
                        break;
                    }

                    if (buyOrder.Remaining == Sell.Remaining) // eşitse ikisini de kapat ve komisyonu kesip şirket hesabına ekle
                    {
                        buyOrder.Status = Types.OrderStatus.Completed.ToString();
                        buyOrder.Collected = buyOrder.Amount;
                        buyOrder.Remaining = 0;
                        buyOrder.CommissionFee = buyOrder.Collected * crypto.Commission;
                        buyOrder.UpdatedDate = DateTime.Now;

                        Sell.Status = Types.OrderStatus.Completed.ToString();
                        Sell.Collected = Sell.Amount;
                        Sell.Remaining = 0;
                        Sell.CommissionFee = Sell.Collected * crypto.Commission;
                        Sell.UpdatedDate = DateTime.Now;
                        _orderService.UpdateSellOrder(Sell);

                        var SellUsdtWallet = _walletDal.Get(w => w.Status == true && w.UserId == Sell.UserId && w.Type == Types.Parities.USDT.ToString());

                        SellUsdtWallet.Balance += Sell.Total - Sell.Price * Sell.CommissionFee;
                        SellUsdtWallet.UpdatedDate = DateTime.Now;
                        _walletDal.Update(SellUsdtWallet);

                        companyUSDTWallet.Total += buyOrder.CommissionFee * buyOrder.Price;
                        _companyWalletDal.Update(companyUSDTWallet);

                        companyCryptoWallet.Total += Sell.CommissionFee;
                        _companyWalletDal.Update(companyCryptoWallet);
                        break;
                    }
                }
            }

            _orderService.CreateBuyOrder(buyOrder);

            if (buyOrder.Status == Types.OrderStatus.Waiting.ToString())//alım emri tamamlanmadı ise sadece cüzdandan bakiyeyi düş
            {
                UsdtWallet.Balance -= buyCryptoDto.Price * buyCryptoDto.Amount;
                UsdtWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(UsdtWallet);
                return new SuccessDataResult<int>(buyOrder.Id, Messages.BuyOrdered);

            }

            //completed ise bunu yap
            {
                UsdtWallet.Balance -= buyCryptoDto.Price * buyCryptoDto.Amount;
                UsdtWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(UsdtWallet);

                buyOrder.Amount -= crypto.Commission * buyCryptoDto.Amount;
                buyOrder.UpdatedDate = DateTime.Now;

                cryptoWallet.Balance += buyOrder.Amount;
                cryptoWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(cryptoWallet);

                return new SuccessDataResult<int>(buyOrder.Id, Messages.BuyOrderCompleted);
            }



        }

        public async Task<IResult> SellCrypto(SellCryptoDto sellCryptoDto)
        {
            var tokenClaim = _tokenHelper.GetTokenInfo();
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }
            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email && x.Status == true);
            if (user == null)
            {
                return new ErrorDataResult<ChangePasswordDto>(Messages.UserNotFound);
            }
            if (sellCryptoDto.Amount <= 0)
            {
                return new ErrorDataResult<Crypto>(Messages.InvalidAmount);
            }
            if (sellCryptoDto.Price <= 0)
            {
                return new ErrorDataResult<Crypto>(Messages.InvalidPrice);
            }

            var cultureInfo = CultureInfo.InvariantCulture;
            var parity = await _parityService.GetPrice(sellCryptoDto.Id);//api'ye istek at ve anlık fiyatı getir.
            if (!parity.Success)
            {
                return new ErrorDataResult<Crypto>(Messages.ParityNotFound.ToString());
            }
            var checkValue = CheckValue(decimal.Parse(parity.Data.price, cultureInfo), sellCryptoDto.Price);//girilen değer min-max aralıkta mı kontrol et
            if (checkValue.Success == false)
            {
                return new ErrorDataResult<Parity>(checkValue.Message);
            }
            var crypto = _cryptoDal.Get(c => c.Id == sellCryptoDto.Id && c.Status == true);//SQL'den kriptoyu getir
            if (crypto == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CryptoNotFound);
            }
            var cryptoUSDT = _cryptoDal.Get(c => c.Name == Types.Parities.USDT.ToString() && c.Status == true);
            if (cryptoUSDT == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CompanyUsdtWalletNotFound);
            }
            var cryptoWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == crypto.Name);
            if (cryptoWallet == null)
            {
                Wallet wallet1 = new()
                {
                    UserId = user.Id,
                    Type = crypto.Name,
                    Balance = 0,
                    Status = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };
                cryptoWallet = wallet1;
                _walletDal.Add(cryptoWallet);
            }
            var UsdtWallet = _walletDal.Get(w => w.UserId == user.Id && w.Status == true && w.Type == Types.Parities.USDT.ToString());//kullanıcının cüzdanını getir
            if (UsdtWallet == null)
            {
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }
            if (cryptoWallet.Balance < sellCryptoDto.Amount)
            {
                return new ErrorDataResult<Wallet>(Messages.NotEnoughBalance);
            }//bakiye (kullanıcının işlem tutarı + komisyon) değerinden düşük ise
            var companyUSDTWallet = _companyWalletDal.Get(cw => cw.CryptoId == cryptoUSDT.Id);//SQL'den şirket cüzdanını getir
            if (companyUSDTWallet == null)
            {

                CompanyWallet companyWallet1 = new()
                {
                    CryptoId = cryptoUSDT.Id,
                    Total = 0
                };
                companyUSDTWallet = companyWallet1;
                _companyWalletDal.Add(companyUSDTWallet);
            }

            var companyCryptoWallet = _companyWalletDal.Get(cw => cw.CryptoId == crypto.Id);
            decimal remaining = sellCryptoDto.Amount;
            decimal commissionFee = 0;
            var getBuyOrders = _orderService.GetBuyOrders().Data.FindAll(s => s.Status == Types.OrderStatus.Waiting.ToString()
            && s.Price == sellCryptoDto.Price && s.UserId != user.Id).OrderBy(s => s.CreatedDate).ToList();

            SellOrder sellOrder = new()
            {
                UserId = user.Id,
                CryptoId = crypto.Id,
                Parity = parity.Data.symbol,
                ParityPrice = decimal.Parse(parity.Data.price, cultureInfo),
                Price = sellCryptoDto.Price,
                Amount = sellCryptoDto.Amount,
                Total = sellCryptoDto.Price * sellCryptoDto.Amount,
                CommissionFee = crypto.Commission * commissionFee,
                Remaining = remaining,
                Collected = 0,
                Status = Types.OrderStatus.Waiting.ToString(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            if (getBuyOrders.Count > 0)
            {
                foreach (var Buy in getBuyOrders)
                {

                    if (Buy.Remaining < sellOrder.Remaining)//eğer alıştaki kripto satıştaki kriptodan düşük ise
                    {
                        sellOrder.Collected += Buy.Remaining; //bu değeri alım emrindeki toplanan kriptolara ekle
                        sellOrder.CommissionFee = sellOrder.Collected * crypto.Commission;
                        sellOrder.Remaining = sellOrder.Amount - sellOrder.Collected;
                        sellOrder.UpdatedDate = DateTime.Now; // tarihini güncelle


                        Buy.Collected = Buy.Amount;
                        Buy.Status = Types.OrderStatus.Completed.ToString();
                        Buy.Remaining = 0;
                        Buy.UpdatedDate = DateTime.Now;
                        Buy.CommissionFee = crypto.Commission * Buy.Amount;
                        _orderService.UpdateBuyOrder(Buy);

                        var BuyCryptoWallet = _walletDal.Get(w => w.Status == true && w.UserId == Buy.UserId && w.Type == crypto.Name);

                        BuyCryptoWallet.Balance += Buy.Amount - Buy.CommissionFee;
                        BuyCryptoWallet.UpdatedDate = DateTime.Now;
                        _walletDal.Update(BuyCryptoWallet);

                        companyUSDTWallet.Total += Buy.CommissionFee * Buy.Price;
                        _companyWalletDal.Update(companyUSDTWallet);
                    }

                    if (sellOrder.Remaining < Buy.Remaining)
                    {
                        sellOrder.Status = Types.OrderStatus.Completed.ToString();
                        sellOrder.Collected = sellOrder.Amount;
                        sellOrder.Remaining = 0;
                        sellOrder.CommissionFee = sellOrder.Amount * crypto.Commission;
                        sellOrder.UpdatedDate = DateTime.Now;


                        Buy.Collected += sellOrder.Collected;
                        Buy.Remaining = Buy.Amount - Buy.Collected;
                        Buy.CommissionFee = Buy.Collected * crypto.Commission;
                        Buy.UpdatedDate = DateTime.Now;
                        _orderService.UpdateBuyOrder(Buy);


                        companyCryptoWallet.Total += sellOrder.CommissionFee;
                        _companyWalletDal.Update(companyCryptoWallet);
                        break;
                    }

                    if (Buy.Remaining == sellOrder.Remaining)
                    {
                        sellOrder.Status = Types.OrderStatus.Completed.ToString();
                        sellOrder.Collected = sellOrder.Amount;
                        sellOrder.Remaining = 0;
                        sellOrder.CommissionFee = sellOrder.Amount * crypto.Commission;
                        sellOrder.UpdatedDate = DateTime.Now;

                        Buy.Status = Types.OrderStatus.Completed.ToString();
                        Buy.Collected = Buy.Amount;
                        Buy.Remaining = 0;
                        Buy.CommissionFee = Buy.Collected * crypto.Commission;
                        Buy.UpdatedDate = DateTime.Now;
                        _orderService.UpdateBuyOrder(Buy);

                        var BuyCryptoWallet = _walletDal.Get(w => w.Status == true && w.UserId == Buy.UserId && w.Type == crypto.Name);
                        BuyCryptoWallet.Balance += Buy.Amount - Buy.CommissionFee;
                        BuyCryptoWallet.UpdatedDate = DateTime.Now;
                        _walletDal.Update(BuyCryptoWallet);

                        companyUSDTWallet.Total += Buy.CommissionFee * Buy.Price;
                        _companyWalletDal.Update(companyUSDTWallet);

                        companyCryptoWallet.Total += sellOrder.Collected * crypto.Commission;
                        _companyWalletDal.Update(companyCryptoWallet);
                        break;
                    }
                }
            }

            _orderService.CreateSellOrder(sellOrder);

            if (sellOrder.Status == Types.OrderStatus.Waiting.ToString())
            {
                cryptoWallet.Balance -= sellCryptoDto.Amount;
                cryptoWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(cryptoWallet);
                return new SuccessDataResult<int>(sellOrder.Id, Messages.SellOrdered);

            }

            //completed ise bunu yap
            {
                cryptoWallet.Balance -= sellCryptoDto.Amount;
                cryptoWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(cryptoWallet);

                UsdtWallet.Balance += sellOrder.Total - sellOrder.Price * sellOrder.CommissionFee;
                UsdtWallet.UpdatedDate = DateTime.Now;
                _walletDal.Update(UsdtWallet);

                sellOrder.Amount -= crypto.Commission * sellCryptoDto.Amount;
                sellOrder.UpdatedDate = DateTime.Now;

                var getCryptoWallet = _walletDal.Get(w => w.Type == crypto.Name && w.UserId == user.Id && w.Status == true);
                if (getCryptoWallet == null)
                {
                    Wallet wallet1 = new()
                    {
                        UserId = user.Id,
                        Type = crypto.Name,
                        Balance = sellOrder.Amount,
                        Status = true,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                    };
                    _walletDal.Add(wallet1);
                }
                if (getCryptoWallet != null)
                {
                    getCryptoWallet.Balance += sellOrder.Amount;
                    getCryptoWallet.UpdatedDate = DateTime.Now;
                    _walletDal.Update(cryptoWallet);
                }
                return new SuccessDataResult<int>(sellOrder.Id, Messages.SellOrderCompleted);

            }



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

        public IDataResult<List<Crypto>> GetList()
        {
            var result = _cryptoDal.GetList().ToList();
            return new SuccessDataResult<List<Crypto>>(result);
        }
    }
}
