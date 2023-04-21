using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Parity:IEntity
    {
        public string symbol { get; set; }
        public string price { get; set; }
    }
}
