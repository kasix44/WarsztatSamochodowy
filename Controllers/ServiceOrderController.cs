using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;
using WorkshopManager.Mappers;

namespace WorkshopManager.Controllers
{
    [Authorize]
    public class ServiceOrderController : Controller
    {
        private readonly IServiceOrderService _serviceOrderService;
        private readonly IServiceOrderCommentService _commentService;
        private readonly IUsedPartService _usedPartService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ServiceOrderMapper _mapper;
        private readonly JobActivityMapper _jobActivityMapper;

        public ServiceOrderController(
            IServiceOrderService serviceOrderService,
            IServiceOrderCommentService commentService,
            IUsedPartService usedPartService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ServiceOrderMapper mapper,
            JobActivityMapper jobActivityMapper)
        {
            _serviceOrderService = serviceOrderService;
            _commentService = commentService;
            _usedPartService = usedPartService;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _jobActivityMapper = jobActivityMapper;
        }

        // GET: ServiceOrder
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Index(string? licensePlate)
        {
            var ordersQuery = _context.ServiceOrders
                .Include(s => s.AssignedMechanic)
                .Include(s => s.Vehicle)
                .AsQueryable();

            if (!string.IsNullOrEmpty(licensePlate))
            {
                ordersQuery = ordersQuery.Where(s => s.Vehicle.LicensePlate == licensePlate);
            }

            ViewBag.LicensePlates = new SelectList(
                await _context.Vehicles
                    .Select(v => new
                    {
                        v.LicensePlate,
                        Display = v.Brand + " " + v.Model + " (" + v.LicensePlate + ")"
                    }).Distinct().ToListAsync(),
                "LicensePlate", "Display", licensePlate
            );

            var orders = await ordersQuery.ToListAsync();
            var orderDtos = orders.Select(o => _mapper.ToDto(o)).ToList();
            return View(orderDtos);
        }

        // GET: ServiceOrder/Details/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var orderDto = await _serviceOrderService.GetByIdAsync(id.Value);
            if (orderDto == null) return NotFound();

            return View(orderDto);
        }

        // GET: ServiceOrder/Create
        [Authorize(Roles = "Admin,Recepcjonista")]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new ServiceOrderDto());
        }

        // POST: ServiceOrder/Create
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,VehicleId,AssignedMechanicId")] ServiceOrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                await _serviceOrderService.AddAsync(orderDto);
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(orderDto.VehicleId, orderDto.AssignedMechanicId);
            return View(orderDto);
        }

        // GET: ServiceOrder/Edit/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderDto = await _serviceOrderService.GetByIdAsync(id.Value);
            if (orderDto == null) return NotFound();

            LoadDropdowns(orderDto.VehicleId, orderDto.AssignedMechanicId);
            return View(orderDto);
        }

        // POST: ServiceOrder/Edit/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,VehicleId,AssignedMechanicId")] ServiceOrderDto orderDto)
        {
            if (id != orderDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _serviceOrderService.UpdateAsync(orderDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _serviceOrderService.ExistsAsync(orderDto.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(orderDto.VehicleId, orderDto.AssignedMechanicId);
            return View(orderDto);
        }
        
        // GET: ServiceOrder/Delete/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var orderDto = await _serviceOrderService.GetByIdAsync(id.Value);
            if (orderDto == null) return NotFound();

            return View(orderDto);
        }

        // POST: ServiceOrder/Delete/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceOrderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
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
            var orderDto = await _serviceOrderService.GetByIdAsync(id);
            if (orderDto == null)
                return NotFound();

            ViewBag.Parts = new SelectList(_context.Parts, "Id", "Name");
            return View(new UsedPartDto { ServiceOrderId = id });
        }

        // POST: ServiceOrder/AddUsedPart
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsedPart(UsedPartDto usedPartDto)
        {
            if (ModelState.IsValid)
            {
                await _usedPartService.AddAsync(usedPartDto);
                return RedirectToAction("Details", new { id = usedPartDto.ServiceOrderId });
            }

            ViewBag.Parts = new SelectList(_context.Parts, "Id", "Name", usedPartDto.PartId);
            return View(usedPartDto);
        }
        
        // POST: ServiceOrder/DeleteUsedPart
        [Authorize(Roles = "Admin,Recepcjonista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUsedPart(int id, int serviceOrderId)
        {
            await _usedPartService.DeleteAsync(id);
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

            var orderDtos = orders.Select(o => _mapper.ToDto(o)).ToList();
            return View("MyOrders", orderDtos);
        }
        
        // GET: ServiceOrder/MechanicDetails/5
        [Authorize(Roles = "Mechanik")]
        public async Task<IActionResult> MechanicDetails(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var orderDto = await _serviceOrderService.GetByIdAsync(id.Value);
            if (orderDto == null || orderDto.AssignedMechanicId != userId) 
                return Forbid(); // Brak dostępu do cudzych zleceń

            return View("MechanicDetails", orderDto);
        }

        // GET: ServiceOrder/EditJobActivity/5
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> EditJobActivity(int? id)
        {
            if (id == null) return NotFound();

            var activity = await _context.JobActivities.FindAsync(id);
            if (activity == null) return NotFound();

            var activityDto = _jobActivityMapper.ToDto(activity);
            return View(activityDto);
        }

        // POST: ServiceOrder/EditJobActivity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Recepcjonista")]
        public async Task<IActionResult> EditJobActivity(int id, [Bind("Id,Description,LaborCost,ServiceOrderId")] JobActivityDto activityDto)
        {
            if (id != activityDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var activity = _jobActivityMapper.ToEntity(activityDto);
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = activityDto.ServiceOrderId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.JobActivities.Any(e => e.Id == activityDto.Id)) return NotFound();
                    throw;
                }
            }

            return View(activityDto);
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

            var activityDtos = availableActivities.Select(a => _jobActivityMapper.ToDto(a)).ToList();

            var model = new AddExistingJobActivityViewModel
            {
                ServiceOrderId = serviceOrderId,
                AvailableJobActivities = activityDtos
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

            var availableActivities = await _context.JobActivities
                .Where(a => a.ServiceOrderId == null)
                .ToListAsync();

            model.AvailableJobActivities = availableActivities.Select(a => _jobActivityMapper.ToDto(a)).ToList();

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
                var commentDto = new ServiceOrderCommentDto
                {
                    Content = content,
                    CreatedAt = DateTime.Now,
                    ServiceOrderId = serviceOrderId,
                    AuthorId = user.Id,
                    AuthorUserName = user.UserName
                };

                await _commentService.AddAsync(commentDto);
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
            var commentDto = await _commentService.GetByIdAsync(id);
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (commentDto == null || (commentDto.AuthorId != currentUserId && !isAdmin))
                return Forbid();

            return View(commentDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanik,Recepcjonista")]
        public async Task<IActionResult> EditComment(int id, string text)
        {
            var commentDto = await _commentService.GetByIdAsync(id);
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (commentDto == null || (commentDto.AuthorId != currentUserId && !isAdmin))
                return Forbid();

            commentDto.Content = text;
            await _commentService.UpdateAsync(commentDto);
            return RedirectToAction("Details", new { id = commentDto.ServiceOrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanik,Recepcjonista")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var commentDto = await _commentService.GetByIdAsync(id);
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (commentDto == null || (commentDto.AuthorId != currentUserId && !isAdmin))
                return Forbid();

            await _commentService.DeleteAsync(id);
            return RedirectToAction("Details", new { id = commentDto.ServiceOrderId });
        }
    }
}