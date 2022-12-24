using AutoMapper;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities;
using NHASoftware.Entities.Social_Entities;

namespace NHASoftware.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SubscriptionDTO, Subscription>();
            CreateMap<Subscription, SubscriptionDTO>();

            CreateMap<Post, PostDTO>();
            CreateMap<PostDTO, Post>();
        }
    }
}
