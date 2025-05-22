using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoiNghiKhoaHoc.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } 

        [Required]
        public int ConferenceId { get; set; }

        [ForeignKey(nameof(ConferenceId))]
        public virtual Conference Conference { get; set; } 
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
