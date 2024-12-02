using ProductDataAccess.DTOs;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutVM> PrepareCheckoutViewModel(int userId, int voucherAppiedId);
        Task<bool> ProcessOrder(OrderDTO orderDTO, int userId);
    }
}
