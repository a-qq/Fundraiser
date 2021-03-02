using AutoMapper;
using SchoolManagement.Application.Schools.Commands.RegisterSchool;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using System.Linq;

namespace SchoolManagement.Infrastructure.Profiles
{
    public sealed class SchoolProfile : Profile
    {
        public SchoolProfile()
        {
            CreateMap<School, SchoolCreatedDTO>()
                .ForMember(m => m.Headmaster, o => o.MapFrom(src => src.Members.FirstOrDefault()));
        }
    }
}
