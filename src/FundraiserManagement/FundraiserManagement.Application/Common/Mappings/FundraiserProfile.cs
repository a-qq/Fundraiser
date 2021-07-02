using AutoMapper;
using FundraiserManagement.Application.Common.Dtos;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using System;

namespace FundraiserManagement.Application.Common.Mappings
{
    internal sealed class FundraiserProfile : Profile
    {
        public FundraiserProfile()
        {
            CreateMap<Fundraiser, FundraiserDto>()
                .ForMember(dest => dest.State, o => o.MapFrom(
                    src => src.State.ToString().Split('_', StringSplitOptions.RemoveEmptyEntries)[0]))
                .ForMember(dest => dest.IsShared, o => o.MapFrom(
                    src => src.Goal.IsShared));
        }   
    }
}