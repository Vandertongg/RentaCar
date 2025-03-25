using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RentaCarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDefaultIdentity<User>(options =>
{
options.SignIn.RequireConfirmedAccount = false;
options.Password.RequireDigit = true;
options.Password.RequiredLength = 6;
options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RentaCarDbContext>();


builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // Създаване на роли
    string[] roleNames = { "Admin", "User", "MehanicBasic", "MehanicAdvanced", "MehanicSpecialized" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Създаване на администратор
    string adminEmail = "admin@rentacar.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByNameAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "RentaCar",
            LastName = "Admin",
            IdentificationNumber = "1234567890"
        };
        var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    // Създаване на механици
    string mehanicBasicEmail = "mehanic.basic@rentacar.com";
    string mehanicBasicPassword = "MehanicBasic123!";
    string mehanicAdvancedEmail = "mehanic.advanced@rentacar.com";
    string mehanicAdvancedPassword = "MehanicAdvanced123!";
    string mehanicSpecializedEmail = "mehanic.specialized@rentacar.com";
    string mehanicSpecializedPassword = "MehanicSpecialized123!";

    var mehanicBasicUser = await userManager.FindByNameAsync(mehanicBasicEmail);
    if (mehanicBasicUser == null)
    {
        var newMehanicBasic = new User
        {
            UserName = mehanicBasicEmail,
            Email = mehanicBasicEmail,
            EmailConfirmed = true,
            FirstName = "RentaCar",
            LastName = "Mehanic Basic",
            IdentificationNumber = "9876543210"
        };
        var createMehanicBasic = await userManager.CreateAsync(newMehanicBasic, mehanicBasicPassword);
        if (createMehanicBasic.Succeeded)
        {
            await userManager.AddToRoleAsync(newMehanicBasic, "MehanicBasic");
        }
    }

    var mehanicAdvancedUser = await userManager.FindByNameAsync(mehanicAdvancedEmail);
    if (mehanicAdvancedUser == null)
    {
        var newMehanicAdvanced = new User
        {
            UserName = mehanicAdvancedEmail,
            Email = mehanicAdvancedEmail,
            EmailConfirmed = true,
            FirstName = "RentaCar",
            LastName = "Mehanic Advanced",
            IdentificationNumber = "1111111111"
        };
        var createMehanicAdvanced = await userManager.CreateAsync(newMehanicAdvanced, mehanicAdvancedPassword);
        if (createMehanicAdvanced.Succeeded)
        {
            await userManager.AddToRoleAsync(newMehanicAdvanced, "MehanicAdvanced");
        }
    }

    var mehanicSpecializedUser = await userManager.FindByNameAsync(mehanicSpecializedEmail);
    if (mehanicSpecializedUser == null)
    {
        var newMehanicSpecialized = new User
        {
            UserName = mehanicSpecializedEmail,
            Email = mehanicSpecializedEmail,
            EmailConfirmed = true,
            FirstName = "RentaCar",
            LastName = "Mehanic Specialized",
            IdentificationNumber = "2222222222"
        };
        var createMehanicSpecialized = await userManager.CreateAsync(newMehanicSpecialized, mehanicSpecializedPassword);
        if (createMehanicSpecialized.Succeeded)
        {
            await userManager.AddToRoleAsync(newMehanicSpecialized, "MehanicSpecialized");
        }
    }
}



