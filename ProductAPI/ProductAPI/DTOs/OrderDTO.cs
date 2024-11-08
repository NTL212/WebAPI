namespace ProductAPI.DTOs
{
	public class OrderDTO
	{
		public int OrderId { get; set; }
		public int UserId { get; set; }
		public DateTime? OrderDate { get; set; }
		public string? Status { get; set; }
		public decimal? TotalAmount { get; set; }

		public string? ReceverName { get; set; }

		public string? Address { get; set; }

		public string? Note { get; set; }

		public string? PhoneNumber { get; set; }
		public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
	}
}
