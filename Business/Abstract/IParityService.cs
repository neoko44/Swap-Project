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
    public interface IParityService
    {
        IDataResult<List<Crypto>> GetParities();
        Task<IDataResult<Parity>> GetPrice(int id);
    }
}
