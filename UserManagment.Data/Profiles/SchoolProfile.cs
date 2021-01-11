using AutoMapper;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Schools.RegisterSchool;
using System.Linq;

namespace SchoolManagement.Data.Profiles
{
    public class SchoolProfile : Profile
    {
        public SchoolProfile()
        {
            CreateMap<School, SchoolCreatedDTO>()
                .ForMember(m => m.Name, o => o.MapFrom(src => src.Name.Value))
                .ForMember(m => m.Headmaster, o => o.MapFrom(src => src.Members.FirstOrDefault()));
        }
    }
}
