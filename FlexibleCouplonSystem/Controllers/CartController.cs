﻿using FlexibleCouplonSystem;
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
            if (cartRequest.CouponCode != null && cartRequest.CartItems.Count !< 3)
            {
                decimal discountedPrice = CalculateDiscountedPrice(coupon, cartTotalPrice);
                return Ok(new { success = true, discounted_price = discountedPrice });
            }
            else if(cartRequest.CouponCode != null && cartRequest.CartItems.Count >= 4)
            {
               decimal discountedPrice = CalculateDiscountedPrice(coupon, cartTotalPrice,cartItems);
               return Ok(new { success = true, discounted_price = discountedPrice });

            }
            else { return BadRequest(new { success = false, message = "Coupon is not valid" }); }
        }
        else
        {
            return BadRequest(new { success = false, message = "Coupon is not valid" });
        }
    }
    [HttpPost("create_item")]
    public IActionResult CreateItem([FromBody] ItemDto item)
    {
        try
        {
            if (item == null)
            {
                return BadRequest(new { success = false, message = "Coupon not be null" });
            }
            var newItem = new Item
            {
                Name = item.Name,
                Price = item.Price,
            };
            var createCoupon = _context.Items.Add(newItem);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Coupon created" });
        }
        catch (Exception)
        {

            throw;
        }
    }
    [HttpGet("check_coupon")]
    public IActionResult getCouponByCode(string code)
    {
        try
        {
            if(!string.IsNullOrEmpty(code))
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Code == code);
                return Ok(new { success = true, data = coupon });
            }
            return BadRequest(new { success = true, mesgesa = "Coupon is not valid" });
        }
        catch (Exception)
        {

            throw;
        }
    }
    [HttpPost("create_coupon")]
    public IActionResult ApplyCoupon([FromBody] CreateCoupon coupon)
    {
        try
        {
            if(coupon == null)
            {
                return BadRequest(new { success = false, message = "Coupon not be null" });
            }
            var newCoupon = new Coupon
            {
                Code = coupon.Code,
                TotalPriceThreshold = coupon.TotalPriceThreshold,
                MinItems = coupon.MinItems,
                DiscountAmount = coupon.DiscountAmount,
                DiscountType = coupon.DiscountType,

            };
            var createCoupon = _context.Coupons.Add(newCoupon);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Coupon created" });
        }
        catch (Exception)
        {

            throw;
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
        //var priceItem = cartItems.Where(x => x.Price == coupon.TotalPriceThreshold).ToList();
        //decimal totalPriceOfItemsWithSamePrice = itemsWithSamePrice.Sum(item => item.Price);

        decimal totalCartPrice = cartItems.Sum(item => item.Price);

        if(totalCartPrice != cartTotalPrice) return false;
/*        bool comparePrice = totalCartPrice == cartTotalPrice;
*/
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
    private decimal CalculateDiscountedPrice(Coupon coupon, decimal cartTotalPrice, List<Item> cartItems)
    {
        if (cartTotalPrice > coupon.TotalPriceThreshold && cartItems.Count >= coupon.MinItems)
        {
            switch (coupon.DiscountType.ToLower())
            {
                case "fixed":
                    return Math.Max(0, cartTotalPrice - coupon.DiscountAmount);
                case "percent":
                    return cartTotalPrice * (1 - coupon.DiscountAmount / 100);
                case "mixed":
                    decimal fixedDiscount = Math.Max(0, cartTotalPrice - coupon.DiscountAmount);
                    decimal percentDiscount = cartTotalPrice * (1 - coupon.DiscountAmount / 100);
                    return Math.Max(fixedDiscount, percentDiscount);
                default:
                    throw new ArgumentException("Invalid discount type");
            }
        }
        else
        {
            return cartTotalPrice; // No discount applied if rules are not met
        }
    }
}

