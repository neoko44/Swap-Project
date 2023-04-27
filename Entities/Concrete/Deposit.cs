using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Deposit:IEntity
    {
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
    }
}
