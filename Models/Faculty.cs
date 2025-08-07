using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class Faculty
    {
        [Key]
        [Required]
        [EmailAddress]
        public string Email { get; set; }    // Primary Key and Login ID

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        public string DeptCode { get; set; }

        [Required]
        public string DeptName { get; set; }

        [Required]
        [Phone]
        public string MobileNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
