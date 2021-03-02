using AutoMapper;
using SchoolManagement.Application.Schools;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Application.Common.Mappings.AutoMapper
{
    internal sealed class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<Member, MemberDTO>();
        }
    }
}
