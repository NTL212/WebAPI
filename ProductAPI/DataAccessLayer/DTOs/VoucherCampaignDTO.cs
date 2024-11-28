namespace DataAccessLayer.DTOs
{
    public class VoucherCampaignDTO
    {
        public int? CampaignId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TargetAudience { get; set; }

        public string Status { get; set; }

        public List<VoucherDTO> AssignedVouchers { get; set; } = new List<VoucherDTO>();
    }
}
