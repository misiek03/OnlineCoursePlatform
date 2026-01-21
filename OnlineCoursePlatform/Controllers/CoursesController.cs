using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Data;
using OnlineCoursePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineCoursePlatform.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string searchString)
        {
            var courses = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                
                courses = courses.Where(s => s.Title.ToLower().Contains(searchLower) || (s.Description != null && s.Description.ToLower().Contains(searchLower)));
            }

            return View(await courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }
            
            if (User.IsInRole("Admin"))
            {
                return View(course);
            }
            
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                bool isEnrolled = course.Enrollments != null && course.Enrollments.Any(e => e.UserId == userId);

                if (!isEnrolled)
                {
                    TempData["Message"] = "Musisz zapisać się na kurs, aby zobaczyć jego zawartość (wideo).";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["Message"] = "Zaloguj się i zapisz do tego kursu, aby zobaczyć szczegóły.";
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,CategoryId,YouTubeUrl")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,CategoryId,YouTubeUrl")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (existingEnrollment != null)
            {
                TempData["Message"] = "Już jesteś zapisany na ten kurs!";
                return RedirectToAction(nameof(Index));
            }
            
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                UserId = userId,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sukces! Zapisałeś się na kurs.";
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Wypisałeś się z kursu.";
            }
            else
            {
                TempData["Message"] = "Nie byłeś zapisany na ten kurs.";
            }

            return RedirectToAction(nameof(Index));
        }
        
        [Authorize]
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Category) // Pobieramy też kategorię
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return View(myEnrollments);
        }
    }
}
