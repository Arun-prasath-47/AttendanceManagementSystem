using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AttendanceManagementSystem.Controllers
{
    public class FacultyDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string email)
        {
            ViewBag.Message = $"Welcome, Faculty ({email})!";
            ViewBag.FacultyEmail = email;
            return View();
        }

        // GET: Mark Attendance (only allow today)
        [HttpGet]
        public IActionResult MarkAttendance(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var faculty = _context.Faculties.FirstOrDefault(f => f.Email == email);
            if (faculty == null)
                return NotFound();

            var today = DateTime.Today;
            var students = _context.Students.Where(s => s.DeptCode == faculty.DeptCode).ToList();

            ViewBag.FacultyEmail = faculty.Email;
            ViewBag.Date = today;
            ViewBag.CanMark = DateTime.Now.Date == today;

            return View(students);
        }

        // POST: Mark Attendance (only allow today)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAttendance(string facultyEmail, DateTime date, string[] rollNumbers, string[] statuses)
        {
            if (date.Date != DateTime.Today)
            {
                TempData["Error"] = "Attendance can only be marked for today.";
                return RedirectToAction("MarkAttendance", new { email = facultyEmail });
            }

            if (rollNumbers == null || statuses == null || rollNumbers.Length != statuses.Length)
            {
                TempData["Error"] = "Invalid attendance submission.";
                return RedirectToAction("MarkAttendance", new { email = facultyEmail });
            }

            try
            {
                for (int i = 0; i < rollNumbers.Length; i++)
                {
                    var attendance = _context.Attendances.FirstOrDefault(a =>
                        a.RollNumber == rollNumbers[i] && a.Date == date && a.FacultyEmail == facultyEmail);

                    if (attendance == null)
                    {
                        _context.Attendances.Add(new Attendance
                        {
                            RollNumber = rollNumbers[i],
                            Date = date,
                            FacultyEmail = facultyEmail,
                            Status = statuses[i]
                        });
                    }
                    else
                    {
                        attendance.Status = statuses[i];
                    }
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Attendance marked successfully!";
            }
            catch (Exception ex)
            {
                // Log exception here (not shown)
                TempData["Error"] = "Error saving attendance. Please try again.";
            }

            return RedirectToAction("AttendanceHistory", new { email = facultyEmail });
        }

        // GET: Attendance History
        [HttpGet]
        public IActionResult AttendanceHistory(string email, DateTime? date)
        {
            var query = _context.Attendances.Where(a => a.FacultyEmail == email);

            if (date.HasValue)
                query = query.Where(a => a.Date == date.Value.Date);

            var attendances = query.OrderByDescending(a => a.Date).ToList();

            ViewBag.FacultyEmail = email;
            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd") ?? "";

            return View(attendances);
        }


        // Utility: Fill "Not Marked" entries for yesterday for any missing attendance
        // -- scheduled job, non-web --
        public void FillNotMarkedAttendance(DateTime date)
        {
            var faculties = _context.Faculties.ToList();
            foreach (var faculty in faculties)
            {
                var students = _context.Students.Where(s => s.DeptCode == faculty.DeptCode).ToList();
                foreach (var student in students)
                {
                    bool exists = _context.Attendances.Any(a =>
                        a.RollNumber == student.RollNumber &&
                        a.Date == date &&
                        a.FacultyEmail == faculty.Email);

                    if (!exists)
                    {
                        _context.Attendances.Add(new Attendance
                        {
                            RollNumber = student.RollNumber,
                            FacultyEmail = faculty.Email,
                            Date = date,
                            Status = "Not Marked"
                        });
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}
