﻿using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;

namespace ProductDataAccess.Repositories
{
	public interface IOrderRepository:IRepository<Order>
	{
        Task<ResultVM> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId);

        Task<Order> GetOrderById(int orderId);

        Task<bool> ConfirmOrders(List<int> selectedOrderIds);
    }
}
