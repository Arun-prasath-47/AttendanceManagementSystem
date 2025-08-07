using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Student Home
        public IActionResult Index(string roll)
        {
            ViewBag.Message = $"Welcome, Student ({roll})!";
            ViewBag.Roll = roll;
            return View();
        }

        [HttpGet]
        public IActionResult AttendanceHistory(string roll, int? year, int? month)
        {
            if (string.IsNullOrEmpty(roll))
                return NotFound();

            var now = DateTime.Now;
            year ??= now.Year;
            month ??= now.Month;
            var start = new DateTime(year.Value, month.Value, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var records = _context.Attendances
                .Where(a => a.RollNumber == roll && a.Date >= start && a.Date <= end)
                .ToList();

            // Prepare the status dictionary for the month
            var grid = new Dictionary<DateTime, string>();
            int daysInMonth = DateTime.DaysInMonth(year.Value, month.Value);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var dt = new DateTime(year.Value, month.Value, day);
                var status = records.FirstOrDefault(a => a.Date.Date == dt.Date)?.Status ?? "Not Marked";
                grid[dt] = status;
            }

            // For summary stats
            ViewBag.Present = grid.Values.Count(x => x == "Present");
            ViewBag.Absent = grid.Values.Count(x => x == "Absent");
            ViewBag.Late = grid.Values.Count(x => x == "Late");
            ViewBag.NotMarked = grid.Values.Count(x => x == "Not Marked");

            ViewBag.Roll = roll;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Grid = grid;
            ViewBag.Table = records.OrderByDescending(r => r.Date).ToList();

            return View();
        }

    }
}
