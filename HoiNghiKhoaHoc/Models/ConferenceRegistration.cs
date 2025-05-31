using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoiNghiKhoaHoc.Models
{
    public class ConferenceRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ConferenceId { get; set; }
        public bool IsApproved { get; set; } = false;

        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
        //kiểm tra thanh toán khi đăng ký
        public bool IsPaid { get; set; } = false;
		public DateTime? PaidDate { get; set; }

		[ForeignKey(nameof(ConferenceId))]
        public virtual Conference Conference { get; set; } 

    }
}
