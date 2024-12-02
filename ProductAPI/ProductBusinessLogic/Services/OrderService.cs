using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.ViewModels;

namespace ProductBusinessLogic.Services
{
    public class OrderService : BaseService<Order, OrderDTO>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVoucherUserRepository _voucherUserRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly ICacheService _cacheService;
        public OrderService(IMapper mapper, IOrderRepository orderRepository, IProductRepository productRepository, IVoucherUserRepository voucherUserRepository, IVoucherRepository voucherRepository, ICacheService cacheService) : base(mapper, orderRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _voucherUserRepository = voucherUserRepository;
            _voucherRepository = voucherRepository;
            _cacheService = cacheService;
        }


        public async Task<ResultVM> CreateOrderAsync(OrderDTO orderDto)
        {
            try
            {
                var order = _mapper.Map<Order>(orderDto);
                order.VoucherId = orderDto.VoucherAppliedId;
                order.OrderDate = DateTime.Now;
                order.Status = "Pending";

                if (order.VoucherId != null && order.VoucherId != 0)
                {
                    var voucherUser = await _voucherUserRepository
                        .GetByIdWithIncludeAsync(x => x.VoucherId == order.VoucherId && x.UserId == order.UserId);
                    voucherUser.TimesUsed += 1;
                    _voucherUserRepository.Update(voucherUser);
                }

                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository
                        .GetByIdWithIncludeAsync(c => c.ProductId == item.ProductId);
                    item.Product = product;
                    product.Stock -= item.Quantity;

                    if (product.Stock < 0)
                    {
                        return new ResultVM(false, $"Product {product.ProductName} out of stock");
                    }
                    _productRepository.Update(product);
                }

                var orderCreated = await _orderRepository.CreateOrder(order);
                if (await _orderRepository.SaveChangesAsync())
                {
                    var result= new ResultVM(true, $"Place order #{orderCreated.OrderId} with total=${orderCreated.TotalAmount} successfully");                   
                    result.Order = _mapper.Map<OrderDTO>(orderCreated);
                    return result;
                }
                return new ResultVM(false, "Place order failed");
            }
            catch (Exception ex)
            {
                return new ResultVM(false, "Place order failed");
            }
        }

        public async Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var orderDtos = _mapper.Map<List<OrderDTO>>(orders);
            return orderDtos;
        }

        public async Task<PagedResult<OrderDTO>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetPagedByUserAsync(userId, pageNumber, pageSize);
            var totalRecords = await _orderRepository.CountAsync(o=>o.UserId==userId);
            var orderPaged = new PagedResult<OrderDTO>();
            orderPaged.Items = _mapper.Map<List<OrderDTO>>(orders);
            orderPaged.TotalRecords = totalRecords;
            orderPaged.PageNumber = pageNumber;
            orderPaged.PageSize = pageSize;
            return orderPaged;
        }

        public async Task<OrderDTO> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            var orderDto = _mapper.Map<OrderDTO>(order);
            return orderDto;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
                return false;

            order.Status = "Canceled";

            if (order.VoucherId > 0)
            {
                var voucherUser = await _voucherUserRepository.GetVoucherUser((int)order.UserId, (int)order.VoucherId);
                if (voucherUser.TimesUsed > 0 && voucherUser.TimesUsed <= voucherUser.Quantity)
                {
                    voucherUser.TimesUsed -= 1;
                }
                _voucherUserRepository.Update(voucherUser);
            }

            foreach (var o in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync((int)o.ProductId);
                if (product != null)
                {
                    product.Stock += o.Quantity;
                }
                _productRepository.Update(product);
            }
            _orderRepository.Update(order);
            return await _orderRepository.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrders(List<int> selectedOrderIds)
        {
            var orders = await _orderRepository.GetAllWithPredicateIncludeAsync(o=>selectedOrderIds.Contains(o.OrderId));
            if (!orders.Any())
            {
                return false;
            }
            foreach(var order in orders)
            {
                order.Status = "Confirmed";
                _orderRepository.Update(order);
            }                         
            return await _orderRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<OrderDTO>> GetOrderPagedWithSearch(int pageNumber, int pageSize, string searchKey)
        {
            var searchText  = searchKey.ToLower();
            var totalRecords = await _orderRepository.CountAsync(o => o.PhoneNumber.Contains(searchText) || o.ReceverName.ToLower().Contains(searchText) || o.User.Email.ToLower().Contains(searchText));
            var orders = await _orderRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, o => o.PhoneNumber.Contains(searchText) || o.ReceverName.ToLower().Contains(searchText) || o.User.Email.ToLower().Contains(searchText));
            return new PagedResult<OrderDTO>{
                Items = _mapper.Map<List<OrderDTO>>(orders),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = totalRecords
            };
        }

        public async Task<int> GetOrderCountByUserAsync(int userId)
        {
           return await _orderRepository.CountAsync(o=>o.UserId == userId); 
        }

        public async Task<List<OrderDTO>> GetSelectedOrders(List<int> selectedOrders)
        {
            var orders = await _orderRepository.GetAllWithPredicateIncludeAsync(o=>selectedOrders.Contains(o.OrderId));
            return _mapper.Map<List<OrderDTO>>(orders); 
        }

        public async Task<OperationResult> ConfirmAndInvalidateCacheAsync(List<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
                return new OperationResult(false, "No orders to confirm.");

            var updated = await ConfirmOrders(orderIds);

            if (!updated)
                return new OperationResult(false, "Failed to confirm orders.");

            var orders = await GetSelectedOrders(orderIds);

            foreach (var order in orders)
            {
                var pageCount = await GetOrderCountByUserAsync(order.UserId);
                await _cacheService.InvalidateUserOrdersCacheAsync(order.UserId, pageCount);
            }

            return new OperationResult(true);
        }
    }
}
