using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Session support for active persona switching
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Database Provider (Professional Multi-Provider Engine: SQL Server / SQLite)
var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        var sqlServerConn = builder.Configuration.GetConnectionString("SqlServerConnection");
        options.UseSqlServer(sqlServerConn, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        });
    }
    else
    {
        var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=dnk_portal.db";
        options.UseSqlite(sqliteConn);
    }
});

// Register Application Services
builder.Services.AddScoped<ITariffCalculatorService, TariffCalculatorService>();
builder.Services.AddScoped<IQrBarcodeService, QrBarcodeService>();
builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Auto-seed Database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization/seeding.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
