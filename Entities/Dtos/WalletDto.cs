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
    public class WalletDto:IDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Balance { get; set; }

    }
}
