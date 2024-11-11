using AutoMapper;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;

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
            CreateMap<VoucherCampaign, VoucherCampaignDTO>().ReverseMap();
            CreateMap<OrderVoucher, OrderVoucherDTO>().ReverseMap();

            CreateMap<CartItem, OrderItem>()
		   .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
		   .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
		   .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
		}
	}
}
