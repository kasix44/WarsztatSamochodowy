using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class JobActivityController : Controller
    {
        private readonly IJobActivityService _jobActivityService;

        public JobActivityController(IJobActivityService jobActivityService)
        {
            _jobActivityService = jobActivityService;
        }

        // GLOBALNA LISTA CZYNNOŚCI
        public async Task<IActionResult> Index()
        {
            var activities = await _jobActivityService.GetAllAsync();
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
                await _jobActivityService.CreateAsync(jobActivity);

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
            var activity = await _jobActivityService.GetByIdAsync(id);
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
                await _jobActivityService.UpdateAsync(jobActivity);
                return RedirectToAction(nameof(Index)); // Zawsze wracaj do listy czynności
            }

            return View(jobActivity);
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? serviceOrderId = null)
        {
            await _jobActivityService.DeleteAsync(id);

            if (serviceOrderId.HasValue)
            {
                return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
