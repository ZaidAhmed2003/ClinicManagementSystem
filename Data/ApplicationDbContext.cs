using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
	{
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
		public DbSet<StaffModel> Staff { get; set; } = null!;
		public DbSet<EducationalActivityModel> Educational_Activity { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder builder)
		{

			base.OnModelCreating(builder);


			// Relationship

			builder.Entity<TransactionModel>()
				.HasOne(t => t.Order)
				.WithMany() // If there's no inverse navigation property
				.HasForeignKey(t => t.OrderId)
				.OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete


			builder.Entity<OrderModel>()
				.HasOne(o => o.PaymentDetail)
				.WithMany()  // assuming PaymentDetail doesn't have navigation property back to OrderModel
				.HasForeignKey(o => o.PaymentId)
				.OnDelete(DeleteBehavior.NoAction);  // Disable cascade delete

			builder.Entity<OrderModel>()
				.HasOne(o => o.User)
				.WithMany()  // assuming ApplicationUser doesn't have a navigation property back to OrderModel
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.NoAction);  // Disable cascade delete

			// Add configurations for GUID keys
			builder.Entity<ApplicationUser>()
				.Property(u => u.Id)
				.HasColumnName("UserId");

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