using System.Linq;
using AutoMapper;
using SchoolManagement.Application.Schools.Commands.RegisterSchool;
using SchoolManagement.Domain.SchoolAggregate.Schools;

namespace SchoolManagement.Application.Common.Mappings.AutoMapper
{
    internal sealed class SchoolProfile : Profile
    {
        public SchoolProfile()
        {
            CreateMap<School, SchoolCreatedDTO>()
                .ForMember(m => m.Headmaster, o => o.MapFrom(src => src.Members.FirstOrDefault()));
        }
    }
}