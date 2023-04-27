using Core.Entities.Abstract;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class OrderDto:IDto
    {
        public int OrderId { get; set; }
        public int CryptoId { get; set; }
        public string Parity { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal ParityPrice { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Remaining { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Collected { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public string Status { get; set; }

        public DateTime Date { get; set; }





    }
}
