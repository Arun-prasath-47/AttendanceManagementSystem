using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }  // Primary Key, auto-increment

        [Required]
        [ForeignKey("Student")]
        public string RollNumber { get; set; } // FK to Student

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }     // Date of marking

        [Required]
        [RegularExpression("Present|Absent|NotMarked")]
        public string Status { get; set; }     // Present/Absent/NotMarked

        [Required]
        [ForeignKey("Faculty")]
        public string FacultyEmail { get; set; } // FK to Faculty who marked it
    }
}
