using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using HoiNghiKhoaHoc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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

// 4. Đăng ký Repository và Service
builder.Services.AddScoped<IConferenceRepository, EFConferenceRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IUserRepository, EFUserRepository>();
builder.Services.AddScoped<IConferenceSpeakerRepository, EFConferenceSpeakerRepository>();
builder.Services.AddScoped<IFavoriteRepository, EFFavoriteRepository>();
builder.Services.AddScoped<IRegistrationRepository, EFRegistrationRepository>();
builder.Services.AddScoped<ISpeakerRepository, EFSpeakerRepository>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();

// 5. Cấu hình Rotativa (phải trước builder.Build())
RotativaConfiguration.Setup(
    builder.Environment.WebRootPath,   // IWebHostEnvironment có WebRootPath
    "Rotativa"                       // thư mục con trong wwwroot chứa wkhtmltopdf
);
//RotativaConfiguration.Setup(
//    @"D:\Temp\C# - Web\dacs\HoiNghiKhoaHoc\wwwroot",  // path mới KHÔNG chứa dấu #
//    "Rotativa"
//);

// 6. Middleware
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseWebSockets();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 7. Định tuyến
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Conferences}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conferences}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
