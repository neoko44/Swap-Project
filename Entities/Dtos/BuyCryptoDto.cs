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
    public class BuyCryptoDto:IDto
    {
        public int Id { get; set; }


        [Column(TypeName = "decimal(18,8)")]
        public int Amount { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Price { get; set; }
    }
}
