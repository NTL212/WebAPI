using ProductDataAccess.Models;

namespace ProductDataAccess.DTOs
{
    public class OrderVoucherDTO
    {
        public int OrderID { get; set; }
        public string VoucherCode { get; set; }
        public decimal DiscountApplied { get; set; }
    }
}
