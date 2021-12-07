using AutoMapper;
using NHASoftware.DTOs;
using NHASoftware.Models;

namespace NHASoftware.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Where we create mappings
            //Example CreateMap<Message, MessageDto>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<AuthorDTO, Author>().ForMember(a => a.Id, opt => opt.Ignore());
            CreateMap<BookAuthorsDTO, AuthorBook>();
            CreateMap<AuthorBook, BookAuthorsDTO>();
        }
    }
}
