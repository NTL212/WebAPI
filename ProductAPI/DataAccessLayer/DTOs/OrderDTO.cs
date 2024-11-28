using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
	public class OrderDTO
	{
		public int OrderId { get; set; }
		public int? VoucherAppliedId { get; set; }
		public int UserId { get; set; }
		public DateTime? OrderDate { get; set; }
		public string? Status { get; set; }

        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock TotalAmount be negative.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "SubTotal is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "SubTotal cannot be negative.")]
        public decimal? SubTotal { get; set; }

		public string? ReceverName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "PhoneNumber cannot exceed 200 characters.")]
        public string? Address { get; set; }

		public string? Note { get; set; }

		public VoucherDTO? Voucher { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required.")]
        [StringLength(15, ErrorMessage = "PhoneNumber cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }
		public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
	}
}
