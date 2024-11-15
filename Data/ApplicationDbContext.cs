using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;
using System.Reflection.Emit;

namespace ClinicManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductCategoryModel> Product_Category { get; set; }
        public DbSet<ProductDiscountModel> Product_Discount { get; set; }
        public DbSet<ProductInventoryModel> Product_Inventory { get; set; }
		public DbSet<OrderModel> Orders { get; set; }


		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Specify precision for decimal properties
            builder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 digits in total, 2 after the decimal point

            builder.Entity<ProductDiscountModel>()
                .Property(d => d.DiscountValue)
                .HasPrecision(18, 2); // 18 digits in total, 2 after the decimal point

        }
    }
}