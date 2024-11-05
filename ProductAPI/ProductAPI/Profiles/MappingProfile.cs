using AutoMapper;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Product, ProductDTO>().ReverseMap();
			CreateMap<Category, CategoryDTO>().ReverseMap();
		}
	}
}
