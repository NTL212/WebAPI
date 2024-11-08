namespace ProductAPI.DTOs
{
    public class VoucherDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxUsage { get; set; }

        public int MaxPerUsage { get; set; }
    }
}
