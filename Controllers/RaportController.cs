using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WorkshopManager.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.Models;


namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Generate(string? customerEmail, string? licensePlate, string? month)
        {
            var query = _context.ServiceOrders
                .Include(o => o.Vehicle).ThenInclude(v => v.Customer)
                .Include(o => o.UsedParts).ThenInclude(up => up.Part)
                .Include(o => o.JobActivities)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerEmail))
                query = query.Where(o => o.Vehicle.Customer.Email == customerEmail);

            if (!string.IsNullOrEmpty(licensePlate))
                query = query.Where(o => o.Vehicle.LicensePlate == licensePlate);

            if (!string.IsNullOrEmpty(month) &&
                DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                query = query.Where(o => o.StartDate.Year == date.Year && o.StartDate.Month == date.Month);
            }

            var orders = await query.ToListAsync();

            var totalCost = orders.Sum(o =>
                (o.UsedParts?.Sum(p => (p.Part?.UnitPrice ?? 0) * p.Quantity) ?? 0) +
                (o.JobActivities?.Sum(a => a.LaborCost) ?? 0)
            );

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Raport kosztów napraw").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        foreach (var order in orders)
                        {
                            var cost = (order.UsedParts?.Sum(p => (p.Part?.UnitPrice ?? 0) * p.Quantity) ?? 0)
                                     + (order.JobActivities?.Sum(a => a.LaborCost) ?? 0);

                            col.Item().Text($"{order.Vehicle.Customer.FirstName} {order.Vehicle.Customer.LastName} | {order.Vehicle.LicensePlate} | {order.StartDate:d} – {order.EndDate?.ToShortDateString() ?? "-"} | Koszt: {cost:C}");
                        }

                        col.Item().PaddingTop(20).Text($"Łączny koszt: {totalCost:C}").Bold();
                    });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", "RaportKosztow.pdf");
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Select(c => new SelectListItem
                {
                    Value = c.Email,
                    Text = c.FirstName + " " + c.LastName + " (" + c.Email + ")"
                }).ToListAsync();

            var licensePlates = await _context.Vehicles
                .Select(v => new SelectListItem
                {
                    Value = v.LicensePlate,
                    Text = v.Brand + " " + v.Model + " (" + v.LicensePlate + ")"
                }).ToListAsync();

            var model = new ReportFilterViewModel
            {
                Customers = customers,
                LicensePlates = licensePlates
            };

            return View(model);
        }

    }
}
