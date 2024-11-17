using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;  

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var gymClasses = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .Select(g => new GymClassViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    StartTime = g.StartTime,
                    Duration = g.Duration,
                    Description = g.Description,
                    IsBooked = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                })
                .ToListAsync();

            return View(gymClasses);
        }

        // GET: GymClasses/Details/5
        [Authorize]  
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .Include(g => g.AttendingMembers)
                .ThenInclude(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize]  
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] 
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            ModelState.Remove("AttendingMembers");

            if (ModelState.IsValid)
            {
                if (gymClass.AttendingMembers == null)
                {
                    gymClass.AttendingMembers = new List<ApplicationUserGymClass>();
                }

                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize]  
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize]  
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClass.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]  
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gymClass == null)
            {
                return NotFound();
            }

            var attending = await _context.Set<ApplicationUserGymClass>()
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId && a.GymClassId == id);

            if (attending == null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = gymClass.Id
                };
                _context.Add(booking);
            }
            else
            {
                _context.Remove(attending);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { returnToId = id });
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClass.Any(e => e.Id == id);
        }
    }
}