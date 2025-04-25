using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class PartController : Controller
    {
        private readonly IPartService _partService;

        public PartController(IPartService partService)
        {
            _partService = partService;
        }

        // GET: Part
        public async Task<IActionResult> Index()
        {
            var parts = await _partService.GetAllAsync();
            return View(parts);
        }

        // GET: Part/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var part = await _partService.GetByIdAsync(id.Value);
            if (part == null)
                return NotFound();

            return View(part);
        }

        // GET: Part/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Part/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UnitPrice")] Part part)
        {
            if (ModelState.IsValid)
            {
                await _partService.CreateAsync(part);
                return RedirectToAction(nameof(Index));
            }
            return View(part);
        }

        // GET: Part/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var part = await _partService.GetByIdAsync(id.Value);
            if (part == null)
                return NotFound();

            return View(part);
        }

        // POST: Part/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UnitPrice")] Part part)
        {
            if (id != part.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _partService.UpdateAsync(part);
                }
                catch
                {
                    if (!await _partService.ExistsAsync(part.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(part);
        }

        // GET: Part/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var part = await _partService.GetByIdAsync(id.Value);
            if (part == null)
                return NotFound();

            return View(part);
        }

        // POST: Part/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _partService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
