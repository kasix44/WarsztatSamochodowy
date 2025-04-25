using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;
using WorkshopManager.Mappers;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class PartController : Controller
    {
        private readonly IPartService _partService;
        private readonly PartMapper _mapper;

        public PartController(IPartService partService, PartMapper mapper)
        {
            _partService = partService;
            _mapper = mapper;
        }

        // GET: Part
        public async Task<IActionResult> Index()
        {
            var partDtos = await _partService.GetAllAsync();
            return View(partDtos);
        }

        // GET: Part/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var partDto = await _partService.GetByIdAsync(id.Value);
            if (partDto == null)
                return NotFound();

            return View(partDto);
        }

        // GET: Part/Create
        public IActionResult Create()
        {
            return View(new PartDto());
        }

        // POST: Part/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UnitPrice")] PartDto partDto)
        {
            if (ModelState.IsValid)
            {
                await _partService.AddAsync(partDto);
                return RedirectToAction(nameof(Index));
            }
            return View(partDto);
        }

        // GET: Part/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var partDto = await _partService.GetByIdAsync(id.Value);
            if (partDto == null)
                return NotFound();

            return View(partDto);
        }

        // POST: Part/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UnitPrice")] PartDto partDto)
        {
            if (id != partDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _partService.UpdateAsync(partDto);
                }
                catch
                {
                    if (!await _partService.ExistsAsync(partDto.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(partDto);
        }

        // GET: Part/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var partDto = await _partService.GetByIdAsync(id.Value);
            if (partDto == null)
                return NotFound();

            return View(partDto);
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
