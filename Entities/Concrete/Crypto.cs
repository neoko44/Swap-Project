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
    public class Crypto:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }

        [Range(0, 9999999999.99999999, ErrorMessage = "Geçersiz Değer")]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Commission { get; set; }


    }
}
