using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoiNghiKhoaHoc.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int ConferenceId { get; set; }

        [ForeignKey("ConferenceId")]
        public Conference Conference { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
