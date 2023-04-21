using Core.Utilities.Results;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IResult Deposit(decimal money);
        IResult Withdraw(decimal money);
    }
}
