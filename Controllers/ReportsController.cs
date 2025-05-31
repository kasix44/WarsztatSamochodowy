using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO; // Required for MemoryStream
using Newtonsoft.Json; // Required for serializing/deserializing TempData

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin,Recepcjonista")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceOrderService _serviceOrderService;

        public ReportsController(ApplicationDbContext context, IServiceOrderService serviceOrderService)
        {
            _context = context;
            _serviceOrderService = serviceOrderService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ReportFilterViewModel
            {
                Customers = await _context.Customers
                    .Select(c => new SelectListItem
                    {
                        Value = c.Email,
                        Text = $"{c.FirstName} {c.LastName} ({c.Email})"
                    })
                    .ToListAsync(),
                LicensePlates = await _context.Vehicles
                    .Select(v => new SelectListItem
                    {
                        Value = v.LicensePlate,
                        Text = $"{v.Brand} {v.Model} ({v.LicensePlate})"
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(ReportFilterViewModel model)
        {
            var query = _context.ServiceOrders
                .Include(so => so.Vehicle)
                    .ThenInclude(v => v!.Customer)
                .Include(so => so.AssignedMechanic)
                .Include(so => so.UsedParts!)
                    .ThenInclude(up => up.Part)
                .Include(so => so.JobActivities)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(model.CustomerEmail))
            {
                query = query.Where(so => so.Vehicle!.Customer!.Email == model.CustomerEmail);
            }

            if (!string.IsNullOrEmpty(model.LicensePlate))
            {
                query = query.Where(so => so.Vehicle!.LicensePlate == model.LicensePlate);
            }

            if (!string.IsNullOrEmpty(model.Month))
            {
                if (DateTime.TryParse(model.Month + "-01", out DateTime startDate))
                {
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    query = query.Where(so => so.StartDate >= startDate && so.StartDate <= endDate);
                }
            }

            var orders = await query.ToListAsync();

            // Generate report data
            var reportData = new ReportViewModel
            {
                Orders = orders,
                TotalOrders = orders.Count,
                TotalParts = orders.Sum(o => o.UsedParts?.Count ?? 0),
                TotalActivities = orders.Sum(o => o.JobActivities?.Count ?? 0),
                TotalCost = orders.Sum(o => 
                    (o.UsedParts?.Sum(up => (up.Part?.UnitPrice ?? 0) * up.Quantity) ?? 0) +
                    (o.JobActivities?.Sum(ja => ja.LaborCost) ?? 0)
                )
            };
            
            // Store the model in TempData for PDF export
            TempData["ReportFilter"] = JsonConvert.SerializeObject(model);


            return View("Report", reportData);
        }

        public async Task<IActionResult> ExportPdf()
        {
            ReportFilterViewModel model;
            if (TempData["ReportFilter"] is string serializedModel)
            {
                model = JsonConvert.DeserializeObject<ReportFilterViewModel>(serializedModel)!;
                 // Re-store it in TempData in case the user wants to export again without regenerating
                TempData["ReportFilter"] = serializedModel;
            }
            else
            {
                // Handle the case where TempData is empty or not the expected type
                // Redirect to Index or show an error
                return RedirectToAction(nameof(Index));
            }

            var query = _context.ServiceOrders
                .Include(so => so.Vehicle)
                    .ThenInclude(v => v!.Customer)
                .Include(so => so.AssignedMechanic)
                .Include(so => so.UsedParts!)
                    .ThenInclude(up => up.Part)
                .Include(so => so.JobActivities)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.CustomerEmail))
            {
                query = query.Where(so => so.Vehicle!.Customer!.Email == model.CustomerEmail);
            }

            if (!string.IsNullOrEmpty(model.LicensePlate))
            {
                query = query.Where(so => so.Vehicle!.LicensePlate == model.LicensePlate);
            }

            if (!string.IsNullOrEmpty(model.Month))
            {
                if (DateTime.TryParse(model.Month + "-01", out DateTime startDate))
                {
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    query = query.Where(so => so.StartDate >= startDate && so.StartDate <= endDate);
                }
            }

            var orders = await query.ToListAsync();
            var reportViewModel = new ReportViewModel
            {
                Orders = orders,
                TotalOrders = orders.Count,
                TotalParts = orders.Sum(o => o.UsedParts?.Count ?? 0),
                TotalActivities = orders.Sum(o => o.JobActivities?.Count ?? 0),
                TotalCost = orders.Sum(o => 
                    (o.UsedParts?.Sum(up => (up.Part?.UnitPrice ?? 0) * up.Quantity) ?? 0) +
                    (o.JobActivities?.Sum(ja => ja.LaborCost) ?? 0)
                )
            };

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Raport Zleceń Serwisowych")
                        .SemiBold().FontSize(20).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Summary section
                            column.Item().Row(row => {
                                row.RelativeItem().Text($"Liczba zleceń: {reportViewModel.TotalOrders}");
                                row.RelativeItem().Text($"Liczba części: {reportViewModel.TotalParts}");
                            });
                            column.Item().Row(row => {
                                row.RelativeItem().Text($"Liczba czynności: {reportViewModel.TotalActivities}");
                                row.RelativeItem().Text($"Koszt całkowity: {reportViewModel.TotalCost:C}");
                            });

                            column.Item().PaddingTop(1, Unit.Centimetre); // Add some space before the table


                            // Table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // ID
                                    columns.RelativeColumn(2); // Data rozpoczęcia
                                    columns.RelativeColumn(); // Status
                                    columns.RelativeColumn(3); // Pojazd
                                    columns.RelativeColumn(3); // Klient
                                    columns.RelativeColumn(2); // Mechanik
                                    columns.RelativeColumn(); // Liczba części
                                    columns.RelativeColumn(); // Liczba czynności
                                    columns.RelativeColumn(1.5f); // Koszt
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("ID");
                                    header.Cell().Text("Data rozpoczęcia");
                                    header.Cell().Text("Status");
                                    header.Cell().Text("Pojazd");
                                    header.Cell().Text("Klient");
                                    header.Cell().Text("Mechanik");
                                    header.Cell().Text("Części");
                                    header.Cell().Text("Czynności");
                                    header.Cell().Text("Koszt");
                                });

                                foreach (var order in reportViewModel.Orders)
                                {
                                    table.Cell().Text(order.Id.ToString());
                                    table.Cell().Text(order.StartDate.ToString("yyyy-MM-dd"));
                                    table.Cell().Text(order.Status.ToString());
                                    table.Cell().Text($"{order.Vehicle?.Brand} {order.Vehicle?.Model} ({order.Vehicle?.LicensePlate})");
                                    table.Cell().Text($"{order.Vehicle?.Customer?.FirstName} {order.Vehicle?.Customer?.LastName}");
                                    table.Cell().Text(order.AssignedMechanic?.UserName ?? "N/A");
                                    table.Cell().Text((order.UsedParts?.Count ?? 0).ToString());
                                    table.Cell().Text((order.JobActivities?.Count ?? 0).ToString());
                                    table.Cell().Text(((
                                        (order.UsedParts?.Sum(up => (up.Part?.UnitPrice ?? 0) * up.Quantity) ?? 0) +
                                        (order.JobActivities?.Sum(ja => ja.LaborCost) ?? 0)
                                    )).ToString("C"));
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Data wygenerowania: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            x.EmptyLine();
                            x.Span("Strona ");
                            x.CurrentPageNumber();
                            x.Span(" z ");
                            x.TotalPages();
                        });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0; // Reset stream position to the beginning

            return File(stream, "application/pdf", $"Raport_Zlecen_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
    }
} 