using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class ServiceOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ServiceOrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ServiceOrder
        public async Task<IActionResult> Index()
        {
            var orders = _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle);
            return View(await orders.ToListAsync());
        }

        // GET: ServiceOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceOrder = await _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceOrder == null) return NotFound();

            return View(serviceOrder);
        }

        // GET: ServiceOrder/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: ServiceOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,VehicleId,AssignedMechanicId")] ServiceOrder serviceOrder)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(serviceOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadDropdowns(serviceOrder.VehicleId, serviceOrder.AssignedMechanicId);
            return View(serviceOrder);
        }

        // GET: ServiceOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null) return NotFound();

            LoadDropdowns(serviceOrder.VehicleId, serviceOrder.AssignedMechanicId);
            return View(serviceOrder);
        }

        // POST: ServiceOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,VehicleId,AssignedMechanicId")] ServiceOrder serviceOrder)
        {
            if (id != serviceOrder.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceOrderExists(serviceOrder.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            LoadDropdowns(serviceOrder.VehicleId, serviceOrder.AssignedMechanicId);
            return View(serviceOrder);
        }

        // GET: ServiceOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceOrder = await _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceOrder == null) return NotFound();

            return View(serviceOrder);
        }

        // POST: ServiceOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder != null)
            {
                _context.ServiceOrders.Remove(serviceOrder);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceOrderExists(int id)
        {
            return _context.ServiceOrders.Any(e => e.Id == id);
        }

        // 👇 Metoda pomocnicza do dropdownów
        private void LoadDropdowns(int? selectedVehicleId = null, string? selectedMechanicId = null)
        {
            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles
                    .Include(v => v.Customer)
                    .Select(v => new
                    {
                        v.Id,
                        Display = v.Brand + " " + v.Model + " (" + v.LicensePlate + ")"
                    }),
                "Id", "Display", selectedVehicleId
            );

            // 👇 Pobieramy użytkowników z bazy i filtrujemy ręcznie
            var allUsers = _userManager.Users.ToList();
            var mechanicy = allUsers
                .Where(u => _userManager.IsInRoleAsync(u, "Mechanik").GetAwaiter().GetResult())
                .Select(u => new { u.Id, Display = u.UserName })
                .ToList();

            ViewData["AssignedMechanicId"] = new SelectList(mechanicy, "Id", "Display", selectedMechanicId);
        }
    }
}
