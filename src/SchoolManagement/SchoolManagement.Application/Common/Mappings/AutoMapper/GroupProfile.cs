using AutoMapper;
using SchoolManagement.Application.Schools;
using SchoolManagement.Domain.SchoolAggregate.Groups;

namespace SchoolManagement.Application.Common.Mappings.AutoMapper
{
    internal sealed class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupDTO>();
        }
    }
}