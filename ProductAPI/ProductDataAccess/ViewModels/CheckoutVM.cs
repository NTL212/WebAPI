using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using System.ComponentModel.DataAnnotations;


namespace ProductDataAccess.ViewModels
{
    public class CheckoutVM
    {
        public List<CartItem> cartItems = new List<CartItem>();
        public List<VoucherUserDTO> voucherUserDTOs = new List<VoucherUserDTO>();
        public VoucherDTO voucherApplied { get; set; }

        public int OrderId { get; set; }
        public int VoucherAppliedId { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }

        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock TotalAmount be negative.")]
        public decimal TotalAmount { get; set; }

        public string? ReceverName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "PhoneNumber cannot exceed 200 characters.")]
        public string? Address { get; set; }

        public string? Note { get; set; }

        public VoucherDTO Voucher { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required.")]
        [StringLength(15, ErrorMessage = "PhoneNumber cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        // Subtotal tính từ tổng giá trị của các item trong giỏ hàng
        public decimal SubTotal
        {
            get
            {
                return cartItems.Sum(item => item.Price * item.Quantity);
            }
        }

        public decimal Discount
        {
            get
            {
                if (voucherApplied != null)
                {
                    // Ví dụ: Giảm giá theo phần trăm
                    if (voucherApplied.DiscountType == "Percent")
                    {
                        return SubTotal * (voucherApplied.DiscountValue / 100);
                    }
                    // Hoặc giảm giá theo số tiền cố định
                    else if (voucherApplied.DiscountType == "Amount")
                    {
                        return voucherApplied.DiscountValue;
                    }
                }
                return 0;
            }
        }

        public decimal total => SubTotal - Discount;


    }
}
