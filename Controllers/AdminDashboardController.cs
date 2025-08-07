using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AttendanceManagementSystem.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard landing page
        public IActionResult Index()
        {
            ViewBag.Message = "Welcome, Admin!";
            return View();
        }

        // ---------------- FACULTY MANAGEMENT ----------------

        public IActionResult Faculties(string searchString, string deptFilter, string sortBy, string sortDir)
        {
            var faculties = _context.Faculties.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(searchString))
                faculties = faculties.Where(f =>
                    f.Name.Contains(searchString) ||
                    f.Email.Contains(searchString) ||
                    f.DeptName.Contains(searchString));

            if (!string.IsNullOrEmpty(deptFilter))
                faculties = faculties.Where(f => f.DeptName == deptFilter);

            // Sorting
            sortBy = sortBy ?? "name";
            sortDir = sortDir ?? "asc";

            switch (sortBy.ToLower())
            {
                case "dob":
                    faculties = sortDir == "desc" ? faculties.OrderByDescending(f => f.DOB) : faculties.OrderBy(f => f.DOB);
                    break;
                case "dept":
                    faculties = sortDir == "desc" ? faculties.OrderByDescending(f => f.DeptName) : faculties.OrderBy(f => f.DeptName);
                    break;
                default:
                    faculties = sortDir == "desc" ? faculties.OrderByDescending(f => f.Name) : faculties.OrderBy(f => f.Name);
                    break;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentDept"] = deptFilter;
            ViewData["SortBy"] = sortBy;
            ViewData["SortDir"] = sortDir;
            ViewData["Departments"] = _context.Faculties.Select(f => f.DeptName).Distinct().ToList();

            return View(faculties.ToList());
        }



        // GET: Add Faculty
        public IActionResult AddFaculty()
        {
            return View();
        }

        // POST: Add Faculty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFaculty(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Faculties.Add(faculty);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Faculty added successfully!";
                return RedirectToAction(nameof(Faculties));
            }
            return View(faculty);
        }

        // GET: Edit faculty form
        [HttpGet]
        public IActionResult EditFaculty(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var faculty = _context.Faculties.FirstOrDefault(f => f.Email == id);
            if (faculty == null)
                return NotFound();

            return View(faculty);
        }

        // POST: Save edited faculty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFaculty(Faculty faculty)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Model validation failed. Please make sure all required fields are filled.";
                return View(faculty);
            }

            var existingFaculty = _context.Faculties.FirstOrDefault(f => f.Email == faculty.Email);
            if (existingFaculty == null)
                return NotFound();

            existingFaculty.Name = faculty.Name;
            existingFaculty.DOB = faculty.DOB;
            existingFaculty.DeptCode = faculty.DeptCode;
            existingFaculty.DeptName = faculty.DeptName;
            existingFaculty.MobileNumber = faculty.MobileNumber;
            // If password is missing from edit view, ensure you pass it as hidden field

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Faculty updated successfully!";
            return RedirectToAction(nameof(Faculties));
        }

        // GET: Confirm delete faculty
        [HttpGet]
        public IActionResult DeleteFaculty(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var faculty = _context.Faculties.FirstOrDefault(f => f.Email == id);
            if (faculty == null)
                return NotFound();

            return View(faculty);
        }

        // POST: Delete faculty confirmed
        [HttpPost, ActionName("DeleteFaculty")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFacultyConfirmed(string id)
        {
            var faculty = _context.Faculties.FirstOrDefault(f => f.Email == id);
            if (faculty == null)
                return NotFound();

            _context.Faculties.Remove(faculty);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Faculty deleted successfully!";
            return RedirectToAction(nameof(Faculties));
        }

        // ---------------- STUDENT MANAGEMENT ----------------

        // List all students
        public IActionResult Students(string searchString, string deptFilter, string sortBy, string sortDir)
        {
            var students = _context.Students.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(searchString))
                students = students.Where(s =>
                    s.Name.Contains(searchString) ||
                    s.RollNumber.Contains(searchString) ||
                    s.DeptName.Contains(searchString));

            if (!string.IsNullOrEmpty(deptFilter))
                students = students.Where(s => s.DeptName == deptFilter);

            // Sorting
            sortBy = sortBy ?? "name";
            sortDir = sortDir ?? "asc";

            switch (sortBy.ToLower())
            {
                case "dob":
                    students = sortDir == "desc" ? students.OrderByDescending(s => s.DOB) : students.OrderBy(s => s.DOB);
                    break;
                case "dept":
                    students = sortDir == "desc" ? students.OrderByDescending(s => s.DeptName) : students.OrderBy(s => s.DeptName);
                    break;
                default: // "name"
                    students = sortDir == "desc" ? students.OrderByDescending(s => s.Name) : students.OrderBy(s => s.Name);
                    break;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentDept"] = deptFilter;
            ViewData["SortBy"] = sortBy;
            ViewData["SortDir"] = sortDir;
            ViewData["Departments"] = _context.Students.Select(s => s.DeptName).Distinct().ToList();

            return View(students.ToList());
        }


        // GET: Add Student
        [HttpGet]
        public IActionResult AddStudent()
        {
            return View();
        }

        // POST: Add Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToAction(nameof(Students));
            }
            return View(student);
        }

        // GET: Edit Student
        [HttpGet]
        public IActionResult EditStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = _context.Students.FirstOrDefault(s => s.RollNumber == id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: Save edited student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Model validation failed. Please make sure all required fields are filled.";
                return View(student);
            }

            var existingStudent = _context.Students.FirstOrDefault(s => s.RollNumber == student.RollNumber);
            if (existingStudent == null)
                return NotFound();

            existingStudent.Name = student.Name;
            existingStudent.DOB = student.DOB;
            existingStudent.DeptCode = student.DeptCode;
            existingStudent.DeptName = student.DeptName;
            existingStudent.MobileNumber = student.MobileNumber;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Student updated successfully!";
            return RedirectToAction(nameof(Students));
        }

        // GET: Confirm delete student
        [HttpGet]
        public IActionResult DeleteStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = _context.Students.FirstOrDefault(s => s.RollNumber == id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: Delete student confirmed
        [HttpPost, ActionName("DeleteStudent")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStudentConfirmed(string id)
        {
            var student = _context.Students.FirstOrDefault(s => s.RollNumber == id);
            if (student == null)
                return NotFound();

            _context.Students.Remove(student);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Student deleted successfully!";
            return RedirectToAction(nameof(Students));
        }
    }
}