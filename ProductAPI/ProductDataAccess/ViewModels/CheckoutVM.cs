using ProductDataAccess.DTOs;
using ProductDataAccess.Models;


namespace ProductDataAccess.ViewModels
{
    public class CheckoutVM
    {
        public List<CartItem> cartItems = new List<CartItem>();
        public List<VoucherUserDTO> voucherUserDTOs = new List<VoucherUserDTO>();
        public VoucherDTO voucherApplied { get; set; }

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
