using FlexibleCouplonSystem.Models;

namespace FlexibleCouplonSystem
{
    public class CartRequest
    {
        public string CouponCode { get; set; }
        public decimal CartTotalPrice { get; set; }
        public List<Item> CartItems { get; set; }
    }
}


