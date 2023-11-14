using FlexibleCouplonSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FlexibleCouplonSystem.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}

