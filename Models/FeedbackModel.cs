using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class FeedbackModel
	{
		[Key]
		public Guid FeedbackId { get; set; }

		[Required]
		public Guid ProductId { get; set; }

		[ForeignKey("ProductId")]
		public required ProductModel Product { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required]
		public int Rating { get; set; }

		public string? Comment { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
