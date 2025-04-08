using AutoMapper;
using NotificationService.Models;
using NotificationService.Models.Dtos;

namespace NotificationService.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<NotificationRequest, NotificationRequestDto>().ReverseMap();
        }
    }
}
