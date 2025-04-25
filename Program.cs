using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using System.Globalization;
using QuestPDF.Infrastructure;
using WorkshopManager.Services;
using WorkshopManager.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IServiceOrderService, ServiceOrderService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IJobActivityService, JobActivityService>();

// 🔠 Ustawienie kultury "pl-PL"
var cultureInfo = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// 🌍 Konfiguracja lokalizacji żądań (RequestLocalization)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { cultureInfo };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// 🔗 Połączenie z bazą danych
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔐 Konfiguracja Identity + role
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🌐 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization(); // <<< WAŻNE! Middleware do kultury PL

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// ===============================
// SEED RÓL + KONT TESTOWYCH
// ===============================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Mechanik", "Recepcjonista" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Admin
    var adminEmail = "admin@demo.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, "Admin123!");
        await userManager.AddToRoleAsync(user, "Admin");
    }

    // Mechanik
    var mechEmail = "mech@demo.com";
    if (await userManager.FindByEmailAsync(mechEmail) == null)
    {
        var user = new IdentityUser
        {
            UserName = mechEmail,
            Email = mechEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, "Mech123!");
        await userManager.AddToRoleAsync(user, "Mechanik");
    }

    // Recepcjonista
    var recepEmail = "recep@demo.com";
    if (await userManager.FindByEmailAsync(recepEmail) == null)
    {
        var user = new IdentityUser
        {
            UserName = recepEmail,
            Email = recepEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, "Recep123!");
        await userManager.AddToRoleAsync(user, "Recepcjonista");
    }
}

app.Run();
