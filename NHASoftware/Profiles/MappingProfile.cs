using AutoMapper;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Entities.Social_Entities;

namespace NHASoftware.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDTO>();
            CreateMap<PostDTO, Post>();

            CreateMap<FriendRequestDTO, FriendRequest>();
            CreateMap<FriendRequest, FriendRequestDTO>();
        }
    }
}
