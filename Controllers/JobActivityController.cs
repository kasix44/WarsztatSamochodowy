using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;
using WorkshopManager.Mappers;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class JobActivityController : Controller
    {
        private readonly IJobActivityService _jobActivityService;
        private readonly JobActivityMapper _mapper;

        public JobActivityController(IJobActivityService jobActivityService, JobActivityMapper mapper)
        {
            _jobActivityService = jobActivityService;
            _mapper = mapper;
        }

        // GLOBALNA LISTA CZYNNOŚCI
        public async Task<IActionResult> Index()
        {
            var activityDtos = await _jobActivityService.GetAllAsync();
            return View(activityDtos);
        }

        // CREATE GLOBAL / DO ZLECENIA

        // GET: JobActivity/Create
        public IActionResult Create(int? serviceOrderId = null)
        {
            var model = new JobActivityDto
            {
                ServiceOrderId = serviceOrderId
            };
            return View(model);
        }

        // POST: JobActivity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobActivityDto activityDto)
        {
            if (ModelState.IsValid)
            {
                await _jobActivityService.AddAsync(activityDto);

                if (activityDto.ServiceOrderId.HasValue)
                {
                    return RedirectToAction("Details", "ServiceOrder", new { id = activityDto.ServiceOrderId });
                }

                return RedirectToAction(nameof(Index));
            }
            return View(activityDto);
        }

        // EDIT

        // GET: JobActivity/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var activityDto = await _jobActivityService.GetByIdAsync(id);
            if (activityDto == null)
                return NotFound();

            return View(activityDto);
        }

        // POST: JobActivity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobActivityDto activityDto)
        {
            if (ModelState.IsValid)
            {
                await _jobActivityService.UpdateAsync(activityDto);
                return RedirectToAction(nameof(Index)); // Zawsze wracaj do listy czynności
            }

            return View(activityDto);
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
