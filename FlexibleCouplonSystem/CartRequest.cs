using FlexibleCouplonSystem.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlexibleCouplonSystem
{
    public class CartRequest
    {
        public string CouponCode { get; set; }
        public decimal CartTotalPrice { get; set; }
        public List<Item> CartItems { get; set; }
    }

    public class CreateCoupon
    {
        public string Code { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPriceThreshold { get; set; }
        public int MinItems { get; set; }
        public string DiscountType { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal DiscountAmount { get; set; }
    }

}


