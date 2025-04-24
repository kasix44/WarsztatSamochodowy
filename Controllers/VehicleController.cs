using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using Microsoft.AspNetCore.Authorization;
using ImageMagick;


namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicle
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicle/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicle/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName");
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,VIN,LicensePlate,CustomerId")] Vehicle vehicle, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                var validationResult = await SaveImageAndReturnPathAsync(ImageFile);
                if (validationResult != null)
                {
                    vehicle.ImagePath = validationResult;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Dozwolone formaty: .jpg, .jpeg, .png, .heic (max 5MB)");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName", vehicle.CustomerId);
            return View(vehicle);
        }


        // GET: Vehicle/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName", vehicle.CustomerId);
            return View(vehicle);
        }

        // POST: Vehicle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,VIN,LicensePlate,ImagePath,CustomerId")] Vehicle vehicle, IFormFile ImageFile)
        {
            if (id != vehicle.Id)
                return NotFound();

            if (ImageFile != null)
            {
                var validationResult = await SaveImageAndReturnPathAsync(ImageFile);
                if (validationResult != null)
                {
                    vehicle.ImagePath = validationResult;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Dozwolone formaty: .jpg, .jpeg, .png, .heic (max 5MB)");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName", vehicle.CustomerId);
            return View(vehicle);
        }

        // GET: Vehicle/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
        
        // 🧩 Pomocnicza metoda zapisująca zdjęcie i obsługująca konwersję
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
