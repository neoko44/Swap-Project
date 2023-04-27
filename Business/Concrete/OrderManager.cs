using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {
        ITokenHelper _tokenHelper;
        IUserDal _userDal;
        IBuyOrderDal _buyOrderDal;
        ISellOrderDal _sellOrderDal;
        ICompanyWalletService _companyWalletService;
        IWalletService _walletService;
        ICryptoDal _cryptoDal;

        public OrderManager(ITokenHelper tokenHelper, IUserDal userDal, IBuyOrderDal buyOrderDal, ISellOrderDal sellOrderDal,
            ICompanyWalletService companyWalletService, IWalletService walletService, ICryptoDal cryptoDal)
        {
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _buyOrderDal = buyOrderDal;
            _sellOrderDal = sellOrderDal;
            _companyWalletService = companyWalletService;
            _walletService = walletService;
            _cryptoDal = cryptoDal;
        }




        public IResult CreateBuyOrder(BuyOrder buyOrder)
        {
            _walletService.GetWallets();
            _companyWalletService.GetById(12);
            _buyOrderDal.Add(buyOrder);
            return new SuccessDataResult<BuyOrder>(Messages.BuyOrdered);
        }
        public IResult UpdateBuyOrder(BuyOrder buyOrder)
        {
            _buyOrderDal.Update(buyOrder);
            return new SuccessDataResult<BuyOrder>(Messages.BuyOrderUpdated);
        }
        public IResult CancelBuyOrder(int orderId)
        {
            var tokenClaim = _tokenHelper.GetTokenInfo();
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }

            var User = _userDal.Get(x => x.Email == tokenClaim.Data.Email && x.Status == true);
            if (User == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }

            var BuyOrder = _buyOrderDal.Get(bo => bo.UserId == User.Id && bo.Id == orderId && bo.Status == Types.OrderStatus.Waiting.ToString());
            if (BuyOrder == null)
            {
                return new ErrorDataResult<BuyOrder>(Messages.BuyOrderNotFound);
            }

            var Crypto = _cryptoDal.GetList().ToList().Find(c => c.Id == BuyOrder.CryptoId);
            if (Crypto == null)
            {
                return new ErrorDataResult<List<Crypto>>(Messages.CryptoNotFound);
            }

            var Usdt = _cryptoDal.GetList().ToList().Find(c => c.Name == Types.Parities.USDT.ToString());
            if (Usdt == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CryptoNotFound);
            }

            var companyCryptoWallet = _companyWalletService.GetById(BuyOrder.CryptoId).Data;
            if (companyCryptoWallet == null)
            {
                CompanyWallet companyWallet = new()
                {
                    CryptoId = BuyOrder.CryptoId,
                    Total = 0
                };
                _companyWalletService.Add(companyWallet);
                companyCryptoWallet = companyWallet;
            }

            var companyUsdtWallet = _companyWalletService.GetById(Usdt.Id).Data;
            if (companyUsdtWallet == null)
            {
                CompanyWallet companyWallet = new()
                {
                    CryptoId = Usdt.Id,
                    Total = 0
                };
                _companyWalletService.Add(companyWallet);
                companyUsdtWallet = companyWallet;
            }

            var userUsdtWallet = _walletService.GetListByUserId(User.Id).Data.Find(uw => uw.Type == Usdt.Name && uw.Status == true);
            if (userUsdtWallet == null)
            {
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }

            BuyOrder.Status = Types.OrderStatus.Cancelled.ToString();
            BuyOrder.UpdatedDate = DateTime.Now;
            _buyOrderDal.Update(BuyOrder);

            companyCryptoWallet.Total += BuyOrder.Collected;
            _companyWalletService.Update(companyCryptoWallet);


            companyUsdtWallet.Total += BuyOrder.Collected * BuyOrder.CommissionFee * BuyOrder.Price;
            _companyWalletService.Update(companyUsdtWallet);


            userUsdtWallet.Balance += BuyOrder.Total - BuyOrder.Price * BuyOrder.CommissionFee;
            _walletService.Update(userUsdtWallet);

            return new SuccessDataResult<OrderDto>(Messages.OrderCancelled);
        }
        public IDataResult<List<BuyOrder>> GetBuyOrders()
        {
            var result = _buyOrderDal.GetList().ToList();
            return new SuccessDataResult<List<BuyOrder>>(result);
        }

        public IDataResult<List<List<OrderDto>>> GetOrders()
        {
            var tokenClaim = _tokenHelper.GetTokenInfo();
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }
            var user = _userDal.Get(x => x.Email == tokenClaim.Data.Email && x.Status == true);
            if (user == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }
            var buyOrders = _buyOrderDal.GetList(b => b.UserId == user.Id).OrderBy(b => b.Status).ToList();
            var sellOrders = _sellOrderDal.GetList(s => s.UserId == user.Id).OrderBy(s => s.Status).ToList();


            List<List<OrderDto>> Orders = new();
            List<OrderDto> buyOrdersDto = new();
            List<OrderDto> sellOrdersDto = new();

            if (buyOrders.Count > 0)
            {
                foreach (var Buy in buyOrders)
                {
                    OrderDto orderDto = new()
                    {
                        OrderId = Buy.Id,
                        Amount = Buy.Amount,
                        Collected = Buy.Collected,
                        CryptoId = Buy.CryptoId,
                        Date = Buy.CreatedDate,
                        Price = Buy.Price,
                        Parity = Buy.Parity,
                        ParityPrice = Buy.ParityPrice,
                        Remaining = Buy.Remaining,
                        Status = Buy.Status,
                        Total = Buy.Total,
                    };
                    buyOrdersDto.Add(orderDto);
                }
                Orders.Add(buyOrdersDto);
            }

            if (sellOrders.Count > 0)
            {
                foreach (var Sell in sellOrders)
                {
                    OrderDto orderDto = new()
                    {
                        OrderId = Sell.Id,
                        Amount = Sell.Amount,
                        Collected = Sell.Collected,
                        CryptoId = Sell.CryptoId,
                        Date = Sell.CreatedDate,
                        Price = Sell.Price,
                        Parity = Sell.Parity,
                        ParityPrice = Sell.ParityPrice,
                        Remaining = Sell.Remaining,
                        Status = Sell.Status,
                        Total = Sell.Total,
                    };
                    sellOrdersDto.Add(orderDto);
                }
                Orders.Add(sellOrdersDto);
            }

            if (Orders.Count == 0)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.NoOrder);
            }

            return new SuccessDataResult<List<List<OrderDto>>>(Orders);

        }

        public IResult CreateSellOrder(SellOrder sellOrder)
        {
            _sellOrderDal.Add(sellOrder);
            return new SuccessDataResult<SellOrder>(Messages.SellOrdered);
        }
        public IResult UpdateSellOrder(SellOrder sellOrder)
        {
            _sellOrderDal.Update(sellOrder);
            return new SuccessDataResult<SellOrder>(Messages.SellOrderUpdated);
        }
        public IResult CancelSellOrder(int orderId)
        {
            var tokenClaim = _tokenHelper.GetTokenInfo();
            if (tokenClaim.Data == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }

            var User = _userDal.Get(x => x.Email == tokenClaim.Data.Email && x.Status == true);
            if (User == null)
            {
                return new ErrorDataResult<List<List<OrderDto>>>(Messages.UserNotFound);
            }

            var SellOrder = _sellOrderDal.Get(bo => bo.UserId == User.Id && bo.Id == orderId && bo.Status == Types.OrderStatus.Waiting.ToString());
            if (SellOrder == null)
            {
                return new ErrorDataResult<BuyOrder>(Messages.SellOrderNotFound);
            }

            var Crypto = _cryptoDal.GetList().ToList().Find(c => c.Id == SellOrder.CryptoId);
            if (Crypto == null)
            {
                return new ErrorDataResult<List<Crypto>>(Messages.CryptoNotFound);
            }

            var Usdt = _cryptoDal.GetList().ToList().Find(c => c.Name == Types.Parities.USDT.ToString());
            if (Usdt == null)
            {
                return new ErrorDataResult<Crypto>(Messages.CryptoNotFound);
            }

            var companyCryptoWallet = _companyWalletService.GetById(SellOrder.CryptoId).Data;
            if (companyCryptoWallet == null)
            {
                CompanyWallet companyWallet = new()
                {
                    CryptoId = SellOrder.CryptoId,
                    Total = 0
                };
                _companyWalletService.Add(companyWallet);
                companyCryptoWallet = companyWallet;
            }

            var companyUsdtWallet = _companyWalletService.GetById(Usdt.Id).Data;
            if (companyUsdtWallet == null)
            {
                CompanyWallet companyWallet = new()
                {
                    CryptoId = Usdt.Id,
                    Total = 0
                };
                _companyWalletService.Add(companyWallet);
                companyUsdtWallet = companyWallet;
            }

            var userCryptoWallet = _walletService.GetListByUserId(User.Id).Data.Find(uw => uw.Type == Crypto.Name && uw.Status == true);
            if (userCryptoWallet == null)
            {
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }

            SellOrder.Status = Types.OrderStatus.Cancelled.ToString();
            SellOrder.UpdatedDate = DateTime.Now;
            _sellOrderDal.Update(SellOrder);

            companyCryptoWallet.Total += SellOrder.CommissionFee * SellOrder.Collected;
            _companyWalletService.Update(companyCryptoWallet);


            companyUsdtWallet.Total += SellOrder.CommissionFee * SellOrder.Price;
            _companyWalletService.Update(companyUsdtWallet);


            userCryptoWallet.Balance += SellOrder.Amount - SellOrder.CommissionFee;
            _walletService.Update(userCryptoWallet);

            return new SuccessDataResult<OrderDto>(Messages.OrderCancelled);
        }
        public IDataResult<List<SellOrder>> GetSellOrders()
        {
            var result = _sellOrderDal.GetList().ToList();
            return new SuccessDataResult<List<SellOrder>>(result);
        }


    }
}
