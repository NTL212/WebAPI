using AutoMapper;
using Notification.Application.DTOs;
using Notification.Application.Interfaces.Repositories;
using Notification.Application.Interfaces.Services;
using Notification.Domain.Entities;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Notification.Application.Services
{
    public class OrderNoticeService : IOrderNoticeService
    {
        private readonly IOrderNoticeRepository _orderNoticeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderNoticeService> _logger;
        private readonly IValidator<OrderNoticeDTO> _orderNoticeValidator;

        public OrderNoticeService(
            IOrderNoticeRepository orderNoticeRepository,
            IMapper mapper,
            ILogger<OrderNoticeService> logger,
            IValidator<OrderNoticeDTO> orderNoticeValidator) 
        {
            _orderNoticeRepository = orderNoticeRepository;
            _mapper = mapper;
            _logger = logger;
            _orderNoticeValidator = orderNoticeValidator;
        }

        public async Task<OrderNoticeDTO> CreateOrderNotice(OrderNoticeDTO orderNoticeDTO)
        {
            try
            {
                var validationResult = await _orderNoticeValidator.ValidateAsync(orderNoticeDTO);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError("Validation failed: " + errorMessages);
                    throw new CustomException($"Validation failed: {errorMessages}");
                }

                orderNoticeDTO.Created = DateTime.Now;
                orderNoticeDTO.IsSeen = false;
                var notice = _mapper.Map<OrderNotice>(orderNoticeDTO);

                var noticeCreated = await _orderNoticeRepository.AddAsync(notice);
                return _mapper.Map<OrderNoticeDTO>(noticeCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order notice.");
                throw new CustomException("An error occurred while creating the order notice.");
            }
        }

        public async Task<bool> DeleteOrderNotice(string id)
        {
            try
            {
                var notice = await _orderNoticeRepository.GetByIdAsync(id);
                if (notice == null)
                {
                    _logger.LogWarning($"Order notice with ID {id} not found.");
                    return false;
                }

                await _orderNoticeRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order notice.");
                throw new CustomException("An error occurred while deleting the order notice.");
            }
        }

        public async Task<List<OrderNoticeDTO>> GetAllOrderNotices()
        {
            try
            {
                var notices = await _orderNoticeRepository.GetAllAsync();
                return notices.Select(p => _mapper.Map<OrderNoticeDTO>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all order notices.");
                throw new CustomException("An error occurred while fetching order notices.");
            }
        }

        public async Task<List<OrderNoticeDTO>> GetAllOrderNoticesByOrder(int orderId)
        {
            try
            {
                var notices = await _orderNoticeRepository.GetAllByOrderAsync(orderId);
                return notices.Select(p => _mapper.Map<OrderNoticeDTO>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching order notices by order.");
                throw new CustomException("An error occurred while fetching order notices by order.");
            }
        }

        public async Task<List<OrderNoticeDTO>> GetAllOrderNoticesByUser(int userId)
        {
            try
            {
                var notices = await _orderNoticeRepository.GetAllByUserAsync(userId);
                return notices.Select(p => _mapper.Map<OrderNoticeDTO>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching order notices by user.");
                throw new CustomException("An error occurred while fetching order notices by user.");
            }
        }

        public async Task<OrderNoticeDTO> GetOrderNoticeById(string id)
        {
            try
            {
                var notice = await _orderNoticeRepository.GetByIdAsync(id);
                if (notice == null)
                {
                    _logger.LogWarning($"Order notice with ID {id} not found.");
                    return null;
                }

                return _mapper.Map<OrderNoticeDTO>(notice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching order notice by ID.");
                throw new CustomException("An error occurred while fetching order notice by ID.");
            }
        }

        public async Task<OrderNoticeDTO> UpdateOrderNotice(OrderNoticeDTO orderNoticeDTO)
        {
            try
            {
                // Validate input
                var validationResult = await _orderNoticeValidator.ValidateAsync(orderNoticeDTO);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError("Validation failed: " + errorMessages);
                    throw new CustomException($"Validation failed: {errorMessages}");
                }

                var notice = _mapper.Map<OrderNotice>(orderNoticeDTO);
                var noticeUpdated = await _orderNoticeRepository.UpdateAsync(notice);
                return _mapper.Map<OrderNoticeDTO>(noticeUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order notice.");
                throw new CustomException("An error occurred while updating the order notice.");
            }
        }

        public async Task<bool> UpdateOrderNoticeIsSeen(string id)
        {
            try
            {
                var orderNotice = await _orderNoticeRepository.GetByIdAsync(id);
                if (orderNotice == null)
                {
                    _logger.LogWarning($"Order notice with ID {id} not found.");
                    return false;
                }

                orderNotice.IsSeen = true;
                var noticeUpdated = await _orderNoticeRepository.UpdateAsync(orderNotice);
                return noticeUpdated.IsSeen;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the IsSeen flag for order notice.");
                throw new CustomException("An error occurred while updating the IsSeen flag for order notice.");
            }
        }
    }

    // Custom Exception Class for handling specific service errors
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}
