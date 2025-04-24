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
    [Authorize]
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
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Index()
        {
            var orders = _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle);
            return View(await orders.ToListAsync());
        }

        // GET: ServiceOrder/Details/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceOrder = await _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle)
                .Include(s => s.UsedParts) 
                .ThenInclude(up => up.Part)
                .Include(s => s.JobActivities)
                .Include(s => s.Comments) 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceOrder == null) return NotFound();

            return View(serviceOrder);
        }

        // GET: ServiceOrder/Create
        [Authorize(Roles = "Admin,Recepcjonista")]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: ServiceOrder/Create
        [Authorize(Roles = "Admin,Recepcjonista")]
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
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null) return NotFound();

            LoadDropdowns(serviceOrder.VehicleId, serviceOrder.AssignedMechanicId);
            return View(serviceOrder);
        }

        // POST: ServiceOrder/Edit/5
        [Authorize(Roles = "Admin,Recepcjonista")]
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
        [Authorize(Roles = "Admin,Recepcjonista")]
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
        [Authorize(Roles = "Admin,Recepcjonista")]
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
        
        // GET: ServiceOrder/AddUsedPart/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> AddUsedPart(int id)
        {
            var order = await _context.ServiceOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            var viewModel = new AddUsedPartViewModel
            {
                ServiceOrderId = id,
                Parts = new SelectList(_context.Parts, "Id", "Name")
            };

            return View(viewModel);
        }


// POST: ServiceOrder/AddUsedPart
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsedPart(AddUsedPartViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usedPart = new UsedPart
                {
                    ServiceOrderId = model.ServiceOrderId,
                    PartId = model.PartId,
                    Quantity = model.Quantity
                };

                _context.UsedParts.Add(usedPart);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.ServiceOrderId });
            }

            // Przywracamy części do modelu, jeśli jest błąd
            model.Parts = new SelectList(_context.Parts, "Id", "Name", model.PartId);
            return View(model);
        }
        
        //usuwanie czesci 
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUsedPart(int id, int serviceOrderId)
        {
            var part = await _context.UsedParts.FindAsync(id);
            if (part != null)
            {
                _context.UsedParts.Remove(part);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = serviceOrderId });
        }

        // GET: ServiceOrder/MyOrders
        [Authorize(Roles = "Mechanik")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.ServiceOrders
                .Include(s => s.Vehicle)
                .Include(s => s.UsedParts)
                .ThenInclude(up => up.Part)
                .Where(s => s.AssignedMechanicId == userId)
                .ToListAsync();

            return View("MyOrders", orders);
        }
        
        // GET: ServiceOrder/MechanicDetails/5
        [Authorize(Roles = "Mechanik")]
        public async Task<IActionResult> MechanicDetails(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var serviceOrder = await _context.ServiceOrders
                .Include(s => s.Vehicle)
                .Include(s => s.UsedParts)
                .ThenInclude(up => up.Part)
                .Include(s => s.JobActivities)
                .Include(s => s.Comments) 
                .FirstOrDefaultAsync(s => s.Id == id && s.AssignedMechanicId == userId);

            if (serviceOrder == null) return Forbid(); // Brak dostępu do cudzych zleceń

            return View("MechanicDetails", serviceOrder);
        }

        // GET: ServiceOrder/EditJobActivity/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> EditJobActivity(int? id)
        {
            if (id == null) return NotFound();

            var activity = await _context.JobActivities.FindAsync(id);
            if (activity == null) return NotFound();

            return View(activity);
        }

// POST: ServiceOrder/EditJobActivity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> EditJobActivity(int id, [Bind("Id,Description,LaborCost,ServiceOrderId")] JobActivity activity)
        {
            if (id != activity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = activity.ServiceOrderId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.JobActivities.Any(e => e.Id == activity.Id)) return NotFound();
                    throw;
                }
            }

            return View(activity);
        }

// POST: ServiceOrder/DeleteJobActivity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> DeleteJobActivity(int id, int serviceOrderId)
        {
            var activity = await _context.JobActivities.FindAsync(id);
            if (activity != null)
            {
                _context.JobActivities.Remove(activity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = serviceOrderId });
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> AddExistingJobActivity(int serviceOrderId)
        {
            var availableActivities = await _context.JobActivities
                .Where(a => a.ServiceOrderId == null)
                .ToListAsync();

            var model = new AddExistingJobActivityViewModel
            {
                ServiceOrderId = serviceOrderId,
                AvailableJobActivities = availableActivities
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> AddExistingJobActivity(AddExistingJobActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activity = await _context.JobActivities.FindAsync(model.SelectedJobActivityId);
                if (activity != null)
                {
                    activity.ServiceOrderId = model.ServiceOrderId;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Details", new { id = model.ServiceOrderId });
            }

            model.AvailableJobActivities = await _context.JobActivities
                .Where(a => a.ServiceOrderId == null)
                .ToListAsync();

            return View(model);
        }
        [HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin,Recepcjonista,Mechanik")]
public async Task<IActionResult> AddComment(int serviceOrderId, string content)
{
    var user = await _userManager.GetUserAsync(User);

    if (!string.IsNullOrWhiteSpace(content) && user != null)
    {
        var comment = new ServiceOrderComment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            ServiceOrderId = serviceOrderId,
            Author = user.UserName,     // lub user.Email, jeśli wolisz
            AuthorId = user.Id
        };

        _context.ServiceOrderComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    var isMechanic = await _userManager.IsInRoleAsync(user, "Mechanik");

    return RedirectToAction(
        isMechanic ? "MechanicDetails" : "Details",
        new { id = serviceOrderId });
}

[HttpGet]
[Authorize(Roles = "Admin,Mechanik,Recepcjonista")]
public async Task<IActionResult> EditComment(int id)
{
    var comment = await _context.ServiceOrderComments.FindAsync(id);
    var currentUserId = _userManager.GetUserId(User);
    var isAdmin = User.IsInRole("Admin");

    if (comment == null || (comment.AuthorId != currentUserId && !isAdmin))
        return Forbid();

    return View(comment);
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin,Mechanik,Recepcjonista")]
public async Task<IActionResult> EditComment(int id, string text)
{
    var comment = await _context.ServiceOrderComments.FindAsync(id);
    var currentUserId = _userManager.GetUserId(User);
    var isAdmin = User.IsInRole("Admin");

    if (comment == null || (comment.AuthorId != currentUserId && !isAdmin))
        return Forbid();

    comment.Content = text;
    await _context.SaveChangesAsync();
    return RedirectToAction("Details", new { id = comment.ServiceOrderId });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin,Mechanik,Recepcjonista")]
public async Task<IActionResult> DeleteComment(int id)
{
    var comment = await _context.ServiceOrderComments.FindAsync(id);
    var currentUserId = _userManager.GetUserId(User);
    var isAdmin = User.IsInRole("Admin");

    if (comment == null || (comment.AuthorId != currentUserId && !isAdmin))
        return Forbid();

    _context.ServiceOrderComments.Remove(comment);
    await _context.SaveChangesAsync();
    return RedirectToAction("Details", new { id = comment.ServiceOrderId });
}


    }
}