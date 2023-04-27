using Core.Entities.Abstract;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class BuyOrder : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public string Parity { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal ParityPrice { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal CommissionFee { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Remaining { get; set; }

        [Range(0, 9999999999.99999999, ErrorMessage = "Geçersiz Değer")]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Collected { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
