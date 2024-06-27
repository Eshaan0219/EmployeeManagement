using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class AddEmployeeDTO
    {
        public int EmpId { get; set; }
        public string? FName { get; set; }
        public string? Lname { get; set; }
        public string? Email { get; set; }
        public string? PhNo { get; set; }
        public string? City { get; set; }
        public string? PinCode { get; set; }
        public DateTime DOB { get; set; }
        public DateTime HireDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Decimal Salary { get; set; }

    }
}
