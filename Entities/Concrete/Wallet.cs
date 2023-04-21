using Core.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    public class Wallet : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }

        //[Precision(18, 8)]
        [Range(0, 9999999999.99999999, ErrorMessage = "Geçersiz Değer")]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


    }
}
