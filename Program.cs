using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using System.Globalization;
using QuestPDF.Infrastructure;
using WorkshopManager.Services;
using WorkshopManager.Services.Interfaces;
using WorkshopManager.Mappers;
using Serilog;
using Serilog.Events;

//serilog - glowny logger 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Verbose)
    .MinimumLevel.Override("Microsoft.AspNetCore.Identity", LogEventLevel.Verbose)
    .MinimumLevel.Override("System", LogEventLevel.Verbose)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs/workshop-app-.log"),
        rollingInterval: RollingInterval.Hour,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateLogger();

try
{
    Log.Information("Starting WorkshopManager application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
//licencja pdf 
QuestPDF.Settings.License = LicenseType.Community;

//rejestracja serwisów DI 
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IServiceOrderService, ServiceOrderService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IJobActivityService, JobActivityService>();
builder.Services.AddScoped<IUsedPartService, UsedPartService>();
builder.Services.AddScoped<IServiceOrderCommentService, ServiceOrderCommentService>();

//ustawienia kultury 
var cultureInfo = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { cultureInfo };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//polaczenie z baza 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//identity - autoryzacja i role 
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

//mappery 
builder.Services.AddScoped<PartMapper>();
builder.Services.AddScoped<UsedPartMapper>();
builder.Services.AddScoped<ServiceOrderMapper>();
builder.Services.AddScoped<ServiceOrderCommentMapper>();
builder.Services.AddScoped<JobActivityMapper>();
builder.Services.AddScoped<VehicleMapper>();
builder.Services.AddScoped<CustomerMapper>();

//mvc + razor pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//swagger - dokumentacja API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Workshop Manager API",
        Version = "v1",
        Description = "API dla systemu zarządzania warsztatem samochodowym"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();

app.UseStaticFiles();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workshop Manager API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//seed uzytkownikow 
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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
