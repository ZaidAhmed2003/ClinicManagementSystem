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

		public DbSet<ProductModel> Products { get; set; } = null!;
		public DbSet<ProductCategoryModel> Product_Category { get; set; } = null!;
		public DbSet<ProductDiscountModel> Product_Discount { get; set; } = null!;
		public DbSet<ProductInventoryModel> Product_Inventory { get; set; } = null!;
		public DbSet<OrderModel> Orders { get; set; } = null!;
		public DbSet<OrderItemModel> OrderItems { get; set; } = null!;
		public DbSet<PaymentDetailModel> PaymentDetails { get; set; } = null!;
		public DbSet<TransactionModel> Transactions { get; set; } = null!;
		public DbSet<CartModel> Carts { get; set; } = null!;
		public DbSet<CartItemModel> CartItems { get; set; } = null!;
		public DbSet<ContactModel> Contacts { get; set; } = null!;
		public DbSet<FeedbackModel> Feedbacks { get; set; } = null!;
		public DbSet<NotificationModel> Notifications { get; set; } = null!;
		public DbSet<UserAddressModel> UserAddresses { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder builder)
		{
		
			base.OnModelCreating(builder);

			// Configure precision for decimal properties
			builder.Entity<ProductModel>()
				.Property(p => p.Price)
				.HasPrecision(18, 2);

			builder.Entity<ProductDiscountModel>()
				.Property(d => d.DiscountValue)
				.HasPrecision(18, 2);

			builder.Entity<OrderItemModel>()
				.Property(oi => oi.Price)
				.HasPrecision(18, 2);

			builder.Entity<PaymentDetailModel>()
				.Property(pd => pd.Amount)
				.HasPrecision(18, 2);

			builder.Entity<TransactionModel>()
				.Property(t => t.Amount)
				.HasPrecision(18, 2);


		}
	}
}