using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using WorkshopManager.DTOs;
using WorkshopManager.Services.Interfaces;
using ImageMagick;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerService _customerService;

        public VehicleController(IVehicleService vehicleService, ICustomerService customerService)
        {
            _vehicleService = vehicleService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return View(vehicles);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _vehicleService.GetByIdAsync(id.Value);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        public async Task<IActionResult> Create()
        {
            var customers = await _customerService.GetAllAsync();
            ViewData["CustomerId"] = new SelectList(customers, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,VIN,LicensePlate,CustomerId")] VehicleDto vehicleDto, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                var path = await SaveImageAndReturnPathAsync(ImageFile);
                if (path != null)
                    vehicleDto.ImagePath = path;
                else
                    ModelState.AddModelError("ImageFile", "Dozwolone formaty: .jpg, .jpeg, .png, .heic (max 5MB)");
            }

            if (ModelState.IsValid)
            {
                await _vehicleService.AddAsync(vehicleDto);
                return RedirectToAction(nameof(Index));
            }

            var customers = await _customerService.GetAllAsync();
            ViewData["CustomerId"] = new SelectList(customers, "Id", "FirstName", vehicleDto.CustomerId);
            return View(vehicleDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _vehicleService.GetByIdAsync(id.Value);
            if (vehicle == null)
                return NotFound();

            var customers = await _customerService.GetAllAsync();
            ViewData["CustomerId"] = new SelectList(customers, "Id", "FirstName", vehicle.CustomerId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleDto vehicleDto)
        {
            if (id != vehicleDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _vehicleService.UpdateAsync(vehicleDto);
                }
                catch
                {
                    if (!_vehicleService.Exists(vehicleDto.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var customers = await _customerService.GetAllAsync();
            ViewData["CustomerId"] = new SelectList(customers, "Id", "FirstName", vehicleDto.CustomerId);
            return View(vehicleDto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _vehicleService.GetByIdAsync(id.Value);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vehicleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImageAndReturnPathAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".heic" };

            if (!allowedExtensions.Contains(ext) || file.Length > 5 * 1024 * 1024)
                return null;

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var fullPath = Path.Combine(uploads, fileName);

            if (ext == ".heic")
            {
                using var stream = file.OpenReadStream();
                using var image = new MagickImage(stream);
                image.Format = MagickFormat.Jpg;
                await image.WriteAsync(fullPath);
            }
            else
            {
                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }
    }
}
