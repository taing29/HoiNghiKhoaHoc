using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Razor, MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 2. Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cấu hình Identity với ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 4. Đăng ký Repository
builder.Services.AddScoped<IConferenceRepository, EFConferenceRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IUserRepository, EFUserRepository>();
builder.Services.AddScoped<IFavoriteRepository, EFFavoriteRepository>();
builder.Services.AddScoped<IRegistrationRepository, EFRegistrationRepository>();


// 5. Middleware
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 6. Cấu hình định tuyến
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Conferences}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conferences}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
