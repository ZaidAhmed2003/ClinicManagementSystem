using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class UserAddressModel
	{
		[Key]
		public Guid AddressId { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required]
		public required string AddressLine1 { get; set; }

		public string? AddressLine2 { get; set; }

		[Required]
		public string? PostalCode { get; set; }

		[Required]
		public required string Country { get; set; }

		[Required]
		public required string City { get; set; }

		public string? Telephone { get; set; }

		public required string Mobile { get; set; }
	}
}
