using AutoMapper;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Schools;

namespace SchoolManagement.Data.Profiles
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<Member, MemberDTO>()
                .ForMember(m => m.FirstName, o => o.MapFrom(src => src.FirstName.Value))
                .ForMember(m => m.LastName, o => o.MapFrom(src => src.LastName.Value))
                .ForMember(m => m.Email, o => o.MapFrom(src => src.Email.Value))
                .ForMember(m => m.Role, o => o.MapFrom(src => src.Role.ToString()))
                .ForMember(m => m.Gender, o => o.MapFrom(src => src.Gender.ToString()));
        }
    }
}
