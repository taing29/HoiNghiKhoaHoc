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
    
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Category> Categories { get; set; }
		public DbSet<ConferenceImage> ConferenceImages { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<HoiNghiKhoaHoc.Models.UserView> UserView { get; set; } = default!;
		public DbSet<Speaker> Speakers { get; set; }
		public DbSet<ConferenceSpeaker> ConferenceSpeakers { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ConferenceRegistration> ConferenceRegistrations { get; set; }

    }
}
