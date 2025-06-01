using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Mail;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services
{
    public class OpenOrderReportBackgroundService : BackgroundService
    {
        private readonly ILogger<OpenOrderReportBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly string _pdfsDirectory;

        public OpenOrderReportBackgroundService(
            ILogger<OpenOrderReportBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            
            // Create PDFs directory if it doesn't exist
            _pdfsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "PDFs");
            if (!Directory.Exists(_pdfsDirectory))
            {
                Directory.CreateDirectory(_pdfsDirectory);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Open Order Report Background Service is starting.");

            // TimeSpan period = TimeSpan.FromHours(24); // Daily
            TimeSpan period = TimeSpan.FromMinutes(2); // For testing: every 2 minutes

            using var timer = new PeriodicTimer(period);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Open Order Report Background Service is stopping.");
                    break;
                }

                _logger.LogInformation("Generating open orders report...");

                try
                {
                    await GenerateAndSendReport();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while generating or sending open orders report.");
                }
            }
        }

        private async Task GenerateAndSendReport()
        {
            using var scope = _scopeFactory.CreateScope();
            var serviceOrderService = scope.ServiceProvider.GetRequiredService<IServiceOrderService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // For direct access if needed for includes

            var openOrders = await dbContext.ServiceOrders
                .Include(so => so.Vehicle)
                    .ThenInclude(v => v!.Customer) // Assuming Vehicle and Customer are not null for an order
                .Include(so => so.AssignedMechanic)
                .Where(so => so.Status == OrderStatus.WTrakcie)
                .ToListAsync();

            if (!openOrders.Any())
            {
                _logger.LogInformation("No open orders to report.");
                return;
            }

            // Generate unique filename with timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var pdfPath = Path.Combine(_pdfsDirectory, $"open_orders_report_{timestamp}.pdf");
            
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Raport Otwartych Zleceń Naprawczych")
                        .SemiBold().FontSize(20).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            foreach (var order in openOrders)
                            {
                                column.Item().Table(table => 
                                {
                                    table.ColumnsDefinition(columns => 
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn(3);
                                    });

                                    table.Cell().Text("Zlecenie ID:");
                                    table.Cell().Text(order.Id.ToString());
                                    
                                    table.Cell().Text("Data Rozpoczęcia:");
                                    table.Cell().Text(order.StartDate.ToString("yyyy-MM-dd HH:mm"));

                                    table.Cell().Text("Pojazd:");
                                    table.Cell().Text($"{order.Vehicle?.Brand} {order.Vehicle?.Model} ({order.Vehicle?.LicensePlate})");

                                    table.Cell().Text("Klient:");
                                    table.Cell().Text($"{order.Vehicle?.Customer?.FirstName} {order.Vehicle?.Customer?.LastName}");

                                    table.Cell().Text("Mechanik:");
                                    table.Cell().Text(order.AssignedMechanic?.UserName ?? "Nieprzypisany");
                                });
                                column.Item().LineHorizontal(1f);
                            }
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
            })
            .GeneratePdf(pdfPath);

            _logger.LogInformation($"Open orders report generated: {pdfPath}");

            await SendEmailWithAttachment(pdfPath);
        }

        private async Task SendEmailWithAttachment(string attachmentPath)
        {
            var emailSettings = _configuration.GetSection("SmtpSettings");
            string smtpServer = emailSettings["Server"]!;
            int smtpPort = int.Parse(emailSettings["Port"]!);
            string smtpUser = emailSettings["User"]!;
            string smtpPass = emailSettings["Pass"]!;
            bool useSsl = bool.Parse(emailSettings["UseSsl"] ?? "true");

            string adminEmail = _configuration["AdminEmail"]!;
            if (string.IsNullOrEmpty(adminEmail))
            {
                _logger.LogError("AdminEmail is not configured. Cannot send report.");
                return;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Raporty WorkshopManager"),
                Subject = "Raport Otwartych Zleceń Naprawczych - " + DateTime.Now.ToString("yyyy-MM-dd"),
                Body = "W załączniku znajduje się automatycznie wygenerowany raport otwartych zleceń naprawczych.",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(adminEmail);
            mailMessage.Attachments.Add(new Attachment(attachmentPath));

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = useSsl,
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Report email sent successfully to {adminEmail}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send report email.");
            }
        }
    }
} 