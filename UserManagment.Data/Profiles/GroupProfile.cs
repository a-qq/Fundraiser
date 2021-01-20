using AutoMapper;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Data.Schools;

namespace SchoolManagement.Data.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupDTO>();
        }
    }
}
