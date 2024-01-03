using AutoMapper;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Entities.Social_Entities;
namespace NHA.Website.Software.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Post, PostDTO>();
        CreateMap<PostDTO, Post>();

        CreateMap<FriendRequestDTO, FriendRequest>();
        CreateMap<FriendRequest, FriendRequestDTO>();

        CreateMap<ChatMessage, ChatMessageDTO>();
        CreateMap<ChatMessageDTO, ChatMessage>();
    }
}
