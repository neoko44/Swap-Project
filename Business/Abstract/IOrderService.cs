using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IOrderService
    {
        IResult CreateBuyOrder(BuyOrder buyOrder);
        IResult UpdateBuyOrder(BuyOrder buyOrder);
        IResult CancelBuyOrder(int orderId);
        IDataResult<List<BuyOrder>> GetBuyOrders();

        IDataResult<List<List<OrderDto>>> GetOrders();

        IResult CreateSellOrder(SellOrder sellOrder);
        IResult UpdateSellOrder(SellOrder sellOrder);
        IResult CancelSellOrder(int orderId);
        IDataResult<List<SellOrder>> GetSellOrders();



    }
}
