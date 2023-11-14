using FlexibleCouplonSystem;
using FlexibleCouplonSystem.DBContext;
using FlexibleCouplonSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("apply_coupon")]
    public IActionResult ApplyCoupon([FromBody] CartRequest cartRequest)
    {
        string couponCode = cartRequest.CouponCode;
        decimal cartTotalPrice = cartRequest.CartTotalPrice;
        List<Item> cartItems = cartRequest.CartItems;

        var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode);

        if (coupon != null && ValidateCouponRules(coupon, cartTotalPrice, cartItems))
        {
            decimal discountedPrice = CalculateDiscountedPrice(coupon, cartTotalPrice);
            return Ok(new { success = true, discounted_price = discountedPrice });
        }
        else
        {
            return BadRequest(new { success = false, message = "Coupon is not valid" });
        }
    }
    private bool ValidateCouponRules(Coupon coupon, decimal cartTotalPrice, List<Item> cartItems)
    {
        // Ensure coupon and cartItems are not null
        if (coupon == null || cartItems == null)
        {
            return false; // Invalid if coupon or cartItems is null
        }

        // Check if the cart total price exceeds the coupon's total price threshold
        bool meetsTotalPriceCondition = cartTotalPrice >= coupon.TotalPriceThreshold;

        // Check if the number of items in the cart meets the minimum items condition
        bool meetsMinItemsCondition = cartItems.Count >= coupon.MinItems;

        // Return true only if both conditions are met
        return meetsTotalPriceCondition && meetsMinItemsCondition;
    }


    /*private bool ValidateCouponRules(Coupon coupon, decimal cartTotalPrice, List<Item> cartItems)
    {
        return cartTotalPrice > coupon.TotalPriceThreshold &&
               cartItems.Count >= coupon.MinItems;
    }*/

    private decimal CalculateDiscountedPrice(Coupon coupon, decimal cartTotalPrice)
    {
        return coupon.DiscountType.ToLower() switch
        {
            "fixed" => Math.Max(0, cartTotalPrice - coupon.DiscountAmount),
            "percent" => cartTotalPrice * (1 - coupon.DiscountAmount / 100),
            _ => cartTotalPrice
        };
    }
}

