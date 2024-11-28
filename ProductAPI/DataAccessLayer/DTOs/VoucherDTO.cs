namespace DataAccessLayer.DTOs
{
    public class VoucherDTO
    {
        public int VoucherId { get; set; }

        public string Code { get; set; } = null!;

        public decimal DiscountValue { get; set; }

        public string DiscountType { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Status { get; set; }

        public int MaxUsage { get; set; }

        public int MaxPerUsage { get; set; }

        public int UsedCount { get; set; }

        public string VoucherType { get; set; }

        public string Conditions { get; set; }

        public int AvailableQuantity => MaxUsage - UsedCount;
        public int ReedemQuantity { get; set; } 
    }
}
