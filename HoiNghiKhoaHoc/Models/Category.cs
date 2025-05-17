using System.ComponentModel.DataAnnotations;

namespace HoiNghiKhoaHoc.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } 
        public List<Conference>? Conference { get; set; }
    }
}
