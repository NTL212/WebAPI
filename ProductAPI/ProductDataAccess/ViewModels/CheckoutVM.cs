using ProductDataAccess.DTOs;
using ProductDataAccess.Models;


namespace ProductDataAccess.ViewModels
{
    public class CheckoutVM
    {
        public List<CartItem> cartItems = new List<CartItem>();
        public List<VoucherUserDTO> voucherUserDTOs = new List<VoucherUserDTO>();
    }
}
