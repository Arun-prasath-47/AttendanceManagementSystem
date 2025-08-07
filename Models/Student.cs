using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class Student
    {
        [Key]
        [Required]
        public string RollNumber { get; set; }  // Primary Key

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
    }
}
