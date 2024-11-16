using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Request;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
            CreateMap<Voucher, VoucherDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<VoucherCampaign, VoucherCampaignDTO>().ReverseMap();
            CreateMap<OrderVoucher, OrderVoucherDTO>().ReverseMap();
            CreateMap<UserGroup, GroupDTO>().ReverseMap();
            CreateMap<Voucher, VoucherCreateVM>().ReverseMap();
            CreateMap<Voucher, VoucherEditVM>().ReverseMap();
            CreateMap<VoucherUser, VoucherUserDTO>().ReverseMap();
        

            CreateMap<PagedResult<Order>, PagedResult<OrderDTO>>()
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PagedResult<Product>, PagedResult<ProductDTO>>()
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PagedResult<Category>, PagedResult<CategoryDTO>>()
          .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PagedResult<User>, PagedResult<UserDTO>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PagedResult<Voucher>, PagedResult<VoucherDTO>>()
          .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, OrderItem>()
           .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
           .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
           .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
