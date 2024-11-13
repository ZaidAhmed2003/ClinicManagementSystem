using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductModel
    {

        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Brand { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ShortDescription { get; set; }

        [Required]
        [MaxLength(100)]
        public required string SKU { get; set; } // Stock Keeping Unit, unique for each product

        [Required, DataType(DataType.Currency)]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000.")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = false;

        public Guid CategoryId { get; set; } // Foreign Key for Category
        [ForeignKey("CategoryId")]
        public required  ProductCategoryModel Category { get; set; } // Navigation property for Category

        public Guid InventoryId { get; set; } // Foreign Key for Inventory
        [ForeignKey("InventoryId")]
        public required ProductInventoryModel Inventory { get; set; } // Navigation property for Inventory

        public Guid? DiscountId { get; set; } // Foreign Key for Category
        [ForeignKey("DiscountId")]
        public  ProductDiscountModel? Discount { get; set; } // Navigation property for Category

        public string? ImagePath { get; set; }

       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; } = null;

    }
}
