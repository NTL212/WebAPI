namespace ProductDataAccess.DTOs
{
	public class CartDTO
	{
		public int CartId { get; set; }
		public int UserId { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
		public decimal Total { get; set; }

		public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
	}
}
