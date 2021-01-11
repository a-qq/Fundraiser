using AutoMapper;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Schools;

namespace SchoolManagement.Data.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(m => m.FirstName, o => o.MapFrom(src => src.FirstName.Value))
                .ForMember(m => m.LastName, o => o.MapFrom(src => src.LastName.Value))
                .ForMember(m => m.Email, o => o.MapFrom(src => src.Email.Value))
                .ForMember(m => m.Role, o => o.MapFrom(src => src.Role.ToString()))
                .ForMember(m => m.Gender, o => o.MapFrom(src => src.Gender.ToString()));
        }
    }
}
