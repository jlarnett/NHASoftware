using AutoMapper;
using NHASoftware.DTOs;
using NHASoftware.Entities;

namespace NHASoftware.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Example CreateMap<Message, MessageDto>();
            CreateMap<SubscriptionDTO, Subscription>();
            CreateMap<Subscription, SubscriptionDTO>();
        }
    }
}
