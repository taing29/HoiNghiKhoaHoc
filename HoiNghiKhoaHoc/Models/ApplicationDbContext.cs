using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //khai báo các bảng trong cơ sở dữ liệu
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
