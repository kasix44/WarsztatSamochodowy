using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class JobActivityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GLOBALNA LISTA CZYNNOŚCI
        public async Task<IActionResult> Index()
        {
            var activities = await _context.JobActivities
                .Include(j => j.ServiceOrder)
                .ToListAsync();
            return View(activities);
        }

        // CREATE GLOBAL / DO ZLECENIA

        // GET: JobActivity/Create
        public IActionResult Create(int? serviceOrderId = null)
        {
            var model = new JobActivity
            {
                ServiceOrderId = serviceOrderId
            };
            return View(model);
        }

        // POST: JobActivity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobActivity jobActivity)
        {
            if (ModelState.IsValid)
            {
                _context.JobActivities.Add(jobActivity);
                await _context.SaveChangesAsync();

                if (jobActivity.ServiceOrderId.HasValue)
                {
                    return RedirectToAction("Details", "ServiceOrder", new { id = jobActivity.ServiceOrderId });
                }

                return RedirectToAction(nameof(Index));
            }
            return View(jobActivity);
        }

        // EDIT

        // GET: JobActivity/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _context.JobActivities.FindAsync(id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

// POST: JobActivity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobActivity jobActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Update(jobActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Zawsze wracaj do listy czynności
            }

            return View(jobActivity);
        }

        // DELETE

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? serviceOrderId = null)
        {
            var activity = await _context.JobActivities.FindAsync(id);
            if (activity != null)
            {
                _context.JobActivities.Remove(activity);
                await _context.SaveChangesAsync();
            }

            if (serviceOrderId.HasValue)
            {
                return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
