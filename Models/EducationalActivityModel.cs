using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
	public class EducationalActivityModel
	{
		[Key]
		public Guid ActivityId { get; set; }

		[Required]
		public string Title { get; set; } // Name or title of the activity
		public string Description { get; set; }

		[Required]
		public string ActivityType { get; set; } // Seminar, Workshop, Practical, etc.

		public DateTime Date { get; set; } // Date of the activity

		public Guid? StaffId { get; set; }
		[ForeignKey("StaffId")]
		public StaffModel Staff { get; set; }

	}
}
