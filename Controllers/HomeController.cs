using System.Diagnostics;
using AttendanceManagementSystem.Models;
using AttendanceManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AttendanceManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: /Home/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        public IActionResult Login(string loginId, string password)
        {
            // Admin hardcoded login
            if ((loginId == "Admin" || loginId == "Admin@example.com") && password == "Admin@123")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            // Faculty login by Email/Password
            var faculty = _context.Faculties.FirstOrDefault(f => f.Email == loginId && f.Password == password);
            if (faculty != null)
            {
                return RedirectToAction("Index", "FacultyDashboard", new { email = faculty.Email });
            }

            // Student login by RollNumber/DOB (unchanged)
            if (DateTime.TryParse(password, out DateTime enteredDob))
            {
                var student = _context.Students.FirstOrDefault(s => s.RollNumber == loginId && s.DOB.Date == enteredDob.Date);
                if (student != null)
                {
                    return RedirectToAction("Index", "StudentDashboard", new { roll = student.RollNumber });
                }
            }

            // Invalid login - show error
            ViewBag.Error = "Invalid User ID and Password";
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // If you implement authentication/cookies, clear them here.
            // For now, just redirect to home.
            return RedirectToAction("Index", "Home");
        }
    }
}
