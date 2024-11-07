using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class StaffModel
    {
        [Key]
        public Guid SatffId { get; set; }
    }
}
