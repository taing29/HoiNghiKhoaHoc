using HoiNghiKhoaHoc.Models;
using HoiNghiKhoaHoc.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
		.AddDefaultTokenProviders()
		.AddDefaultUI()
		.AddEntityFrameworkStores<ApplicationDbContext>();
////Truy cap Identity options
//builder.Services.Configure<IdentityOptions>(options =>
//{
//	// Thiết lập về Password
//	options.Password.RequireDigit = false; // Không bắt phải có số
//	options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
//	options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
//	options.Password.RequireUppercase = false; // Không bắt buộc chữ in
//	options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
//	options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt 

//	// Cấu hình Lockout - khóa user
//	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
//	options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
//	options.Lockout.AllowedForNewUsers = true;

//	// Cấu hình về User.
//	options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
//		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//	options.User.RequireUniqueEmail = true;  // Email là duy nhất


//	// Cấu hình đăng nhập.
//	options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
//	options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
//	options.SignIn.RequireConfirmedAccount = true;

//});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//	options.LoginPath = "/login/";
//	options.LogoutPath = "/logout/";
//	options.AccessDeniedPath = "/khongduoctruycap.html";
//});

//builder.Services.AddAuthentication()
//		.AddGoogle(options =>
//		{
//			var gconfig = builder.Configuration.GetSection("Authentication:Google");
//			options.ClientId = gconfig["ClientId"];
//			options.ClientSecret = gconfig["ClientSecret"];
//			// https://localhost:5001/signin-google
//			options.CallbackPath = "/dang-nhap-tu-google";
//		})
//		.AddFacebook(options =>
//		{
//			var fconfig = builder.Configuration.GetSection("Authentication:Facebook");
//			options.AppId = fconfig["AppId"];
//			options.AppSecret = fconfig["AppSecret"];
//			options.CallbackPath = "/dang-nhap-tu-facebook";
//		})
//		// .AddTwitter()
//		// .AddMicrosoftAccount()
//		;


builder.Services.AddRazorPages();

//c�c services cho c�c repository
builder.Services.AddScoped<IConferenceRepository, EFConferenceRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Conferences}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages();

app.Run();
