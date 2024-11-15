using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class OrderModel
    {
		[Key]
		public Guid OrderId { get; set; }
	}
}
