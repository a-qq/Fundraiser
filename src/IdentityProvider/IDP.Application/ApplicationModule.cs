using Autofac;
using IDP.Application.Common.Options;
using IDP.Application.IntegrationEvents.Handlers;
using IDP.Domain.UserAggregate.Entities;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Reflection;

namespace IDP.Application
{
    public sealed class ApplicationModule : Autofac.Module
    {
        private readonly SecurityCodeOptions _securityCodeOptions;

        public ApplicationModule(SecurityCodeOptions securityCodeOptions)
        {
            _securityCodeOptions = securityCodeOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(FormTutorAssignedIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.RegisterType<PasswordHasher<User>>()
                .As<IPasswordHasher<User>>()
                .InstancePerLifetimeScope();

            builder.RegisterInstance(_securityCodeOptions);
        }
    }
}