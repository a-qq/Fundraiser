using Autofac;
using SchoolManagement.Application;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SchoolManagement.Infrastructure.Services;
using System.Reflection;

namespace SchoolManagement.Infrastructure
{
    public sealed class ProcessingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DomainEventService>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventReducersManager>()
                .AsSelf()
                .WithParameter(new TypedParameter(
                    typeof(Assembly), typeof(School).GetTypeInfo().Assembly))
                .SingleInstance();

            builder.RegisterType<EmailUniquenessChecker>()
                .As<IEmailUniquenessChecker>()
                .InstancePerLifetimeScope();

            builder.RegisterType<LogoStorageService>()
                .As<ILogoStorageService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerDependency();

            builder.RegisterType<IntegrationEventService>()
                .As<IIntegrationEventService>()
                .WithParameter(new TypedParameter(typeof(Assembly), typeof(ApplicationModule).GetTypeInfo().Assembly))
                .WithParameter(new TypedParameter(typeof(string), ApplicationModule.AppName))
                .InstancePerDependency();
        }
    }
}