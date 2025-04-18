using AutoMapper;
using NotificationService.Domain.Entities;
using NotificationService.Models;
using NotificationService.Models.Dtos;

namespace NotificationService.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //NotificationMessage
            CreateMap<NotificationMessage, NotificationMessageDto>().ReverseMap();

            CreateMap<NotificationMessageDto, NotificationRequestDto>().ReverseMap();

            //NotificationRequest
            CreateMap<NotificationRequest, NotificationRequestDto>().ReverseMap();
        }
    }
}
