using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlexibleCouplonSystem.Models
{
  

    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPriceThreshold { get; set; }
        public int MinItems { get; set; }
        public string DiscountType { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal DiscountAmount { get; set; }
    }

}
