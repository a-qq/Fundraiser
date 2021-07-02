using Autofac;
using SchoolManagement.Application.IntegrationEvents.Handlers;
using SchoolManagement.Domain.SchoolAggregate.Schools.EventReducers;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Reflection;
using AutoMapper.Contrib.Autofac.DependencyInjection;

namespace SchoolManagement.Application
{
    public sealed class ApplicationModule : Autofac.Module
    {
        private static readonly string Namespace = typeof(ApplicationModule).Namespace;
        private static readonly int Index = Namespace.IndexOf('.', Namespace.IndexOf('.') + 1);
        public static readonly string AppName = Namespace.Substring(0, Index < 0 ? Namespace.Length : Index);

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(UserRegisteredIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.RegisterAssemblyTypes(typeof(FormTutorDivestedDomainEventReducer).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("EventReducer"))
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterAutoMapper(ThisAssembly);
        }
    }
}