using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public class Types
    {
        public enum OrderStatus { Cancelled = 1, Waiting = 2, Completed = 3 }
        public enum OrderTypes { Buy = 1, Sell = 2 }
        public enum Parities { USDT = 1 }
    }
}