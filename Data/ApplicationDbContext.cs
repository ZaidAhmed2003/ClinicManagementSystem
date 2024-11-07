using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<MedicineModel> Medicines { get; set; }
        public DbSet<ScientificInstrumentModel> ScientificInstruments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<MedicineModel>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)")  // Specifies precision and scale
                    .HasPrecision(18, 2);            // Alternatively, use HasPrecision if only using precision/scale

                // Configure other properties with HasColumnType, HasMaxLength, etc. as needed
                entity.Property(e => e.Code)
                    .HasMaxLength(50);

                entity.Property(e => e.Dosage)
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)"); // For longer text
            });

            // Configure ScientificInstrumentModel similarly
            builder.Entity<ScientificInstrumentModel>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)")  // Ensure precision and scale for price
                    .HasPrecision(18, 2);

                // Add configurations for other properties if necessary
            });
        }
    }
}