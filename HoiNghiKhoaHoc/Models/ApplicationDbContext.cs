using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HoiNghiKhoaHoc.Models;

namespace HoiNghiKhoaHoc.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //khai báo các bảng trong cơ sở dữ liệu
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Category> Categories { get; set; }
		public DbSet<ConferenceImage> ConferenceImages { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<HoiNghiKhoaHoc.Models.UserView> UserView { get; set; } = default!;
		
	}
}
