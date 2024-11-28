using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
	public class StaffModel
	{
		[Key]
		public Guid StaffId { get; set; } // Corrected typo in property name

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Position { get; set; } // Position like Lecturer, Assistant, etc.

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

	}
}
