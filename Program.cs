using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GrapheneTrace.Data.Seeders;
using GrapheneTrace.Data;              
using GrapheneTrace.Areas.Identity.Data;  
using GrapheneTrace.Services; 

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options => {
    options.SignIn.RequireConfirmedAccount = false; 
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IHeatmapService, HeatmapService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>(); 
    

    try
    {
    
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>(); 
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
       var context = services.GetRequiredService<ApplicationDbContext>();

        await ContextSeed.SeedRolesAsync(roleManager);
        await ContextSeed.SeedAdminAsync(userManager, logger); 
       await ContextSeed.SeedPatientsAsync(userManager, logger);
       await ContextSeed.SeedCliniciansAsync(userManager, logger);
       await ContextSeed.SeedHeatmapDataAsync(userManager, context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();