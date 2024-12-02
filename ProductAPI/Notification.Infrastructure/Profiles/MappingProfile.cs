using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Notification.Application.DTOs;
using Notification.Domain.Entities;
using Notification.Infrastructure.Data;

namespace Notification.Infrastructure.Profiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<Notice, OrderNoticeDTO>() 
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created.ToDateTime().ToLocalTime()));
            CreateMap<OrderNoticeDTO, Notice>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created.ToUniversalTime())));

            CreateMap<OrderNoticeDTO, OrderNotice>().ReverseMap();
            CreateMap<OrderNotice, MongoOrderNotice>().ReverseMap()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created.ToLocalTime()));   
        }
    }
}
