using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;

namespace FundraiserManagement.Application
{
    public sealed class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAutoMapper(ThisAssembly);
        }
    }
}
