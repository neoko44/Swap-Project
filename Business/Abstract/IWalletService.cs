using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
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
        IDataResult<WalletDto> Deposit(Deposit deposit);
        IDataResult<List<WalletDto>> GetWallets();
        //IResult Withdraw(Deposit deposit);

        IDataResult<List<Wallet>> GetListByUserId(int userId);

        void Update(Wallet wallet);
    }
}
