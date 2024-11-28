using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public  class VoucherEditVM:VoucherBaseVM
    {
        public int VoucherId { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }
        public string VoucherType { get; set; }
        [Required(ErrorMessage = "Discount Type is required.")]
        public string DiscountType { get; set; }
        [Required(ErrorMessage = "Discount Value is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount Value must be greater than 0.")]
        public decimal DiscountValue { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = " Min Order Value cannot be negative")]
        public decimal? MinOrderValue { get; set; }
        public string? ProductId { get; set; }
        public int? CategoryId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Max Quantity must be greater than 0.")]
        public int? MaxUsage { get; set; }

        public string? Conditions { get; set; }
        public string? GroupName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be negative")]
        public decimal? MaxDiscountAmount { get; set; }
        public string? Status { get; set; }
        public int MaxUsesPerUser { get; set; }
        [Required(ErrorMessage = "Expiry Date is required.")]
        public DateTime ExpiryDate { get; set; }
    }
}
