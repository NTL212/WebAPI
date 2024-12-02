using AutoMapper;
using Notification.Application.DTOs;
using Notification.Domain.Entities;

namespace Notification.Application.Profiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {
            CreateMap<OrderNoticeDTO, OrderNotice>().ReverseMap();
        }
    }
}
