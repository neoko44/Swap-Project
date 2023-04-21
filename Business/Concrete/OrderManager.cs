using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {
        IBuyOrderDal _buyOrderDal;

        public OrderManager(IBuyOrderDal buyOrderDal)
        {
            _buyOrderDal = buyOrderDal;
        }

        public IResult CancelOrder()
        {
            throw new NotImplementedException();
        }

        public IResult CreateBuyOrder(BuyOrder buyOrder)
        {
            _buyOrderDal.Add(buyOrder);
            return new SuccessDataResult<BuyOrder>(Messages.BuyOrdered);
        }

        public IResult CreateSellOrder(SellOrder sellOrder)
        {
            throw new NotImplementedException();
        }
    }
}
