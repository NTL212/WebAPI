using AutoMapper;
using Newtonsoft.Json;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Request;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly RabbitMqService _rabbitMqService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public CheckoutService(
            ICartService cartService, IUserService userService, IVoucherService voucherService,
            RabbitMqService rabbitMqService, IMapper mapper, ICacheService cacheService, IProductService productService)
        {
            _cartService = cartService;
            _userService = userService;
            _voucherService = voucherService;
            _rabbitMqService = rabbitMqService;
            _mapper = mapper;
            _cacheService = cacheService;
            _productService = productService;
        }

        public async Task<CheckoutVM> PrepareCheckoutViewModel(int userId, int voucherAppiedId)
        {
            var cart = _cartService.GetCart();
            var user = await _userService.GetByIdAsync(userId);

            var checkoutVM = new CheckoutVM
            {
                cartItems = cart,
                TotalAmount = cart.Sum(x => x.Quantity * x.Price)
            };

            if (voucherAppiedId > 0)
            {
                var voucher = await _voucherService.GetByIdAsync(voucherAppiedId);
                var validateResult = await _voucherService.ValidateApplyVoucher(
                    _mapper.Map<VoucherDTO>(voucher), user, cart.Select(x => (int)x.ProductId).ToList(), checkoutVM.TotalAmount);

                if (validateResult.Success)
                {
                    checkoutVM.voucherApplied = _mapper.Map<VoucherDTO>(voucher);
                }
            }

            return checkoutVM;
        }
        public async Task<bool> ProcessOrder(OrderDTO orderDTO, int userId)
        {
            var cart = _cartService.GetCart();
            var order = _mapper.Map<Order>(orderDTO);
            foreach (var item in cart) {
                var product = await _productService.GetByIdAsync((int)item.ProductId);
                var newOI = new OrderItem();
                newOI.ProductId = item.ProductId;
                newOI.Quantity = item.Quantity;
                newOI.Price = item.Price;
                newOI.Product = _mapper.Map<Product>(product);
                order.OrderItems.Add(newOI);
            }
            order.UserId = userId;
            order.Status = "Pending";
            order.OrderDate = DateTime.Now;
            var rabbitMessage = new RabbitMessage
            {
                ActionType = "Create",
                Payload = order
            };

            var rabbitMessageJson = JsonConvert.SerializeObject(rabbitMessage);
            _rabbitMqService.PublishOrderMessage(rabbitMessageJson);

            _cartService.ClearCart();
            var pageCount = await _userService.GetCountOrderOfUser(userId);
            _cacheService.InvalidateUserOrdersCacheAsync(userId, pageCount / 5);
            return true;
        }
    }

}
