using Autofac;
using IDP.Application;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using IDP.Infrastructure.Services;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Options;
using System.Reflection;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace IDP.Infrastructure
{
    public sealed class ProcessingModule : Autofac.Module
    {
        private readonly MailOptions _mailSettings;
        private readonly UrlsOptions _urlsSettings;
        private readonly AdministratorsOptions _administratorsOptions;

        public ProcessingModule(
            MailOptions mailSettings,
            UrlsOptions urlsSettings,
            AdministratorsOptions administratorsOptions)
        {
            _mailSettings = mailSettings;
            _urlsSettings = urlsSettings;
            _administratorsOptions = administratorsOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SK.DomainEventService>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<SK.EventReducersManager>()
                .As<IEventReducersManager>()
                .WithParameter(new TypedParameter(
                    typeof(Assembly), typeof(User).GetTypeInfo().Assembly))
                .SingleInstance();

            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerDependency();

            builder.RegisterType<IntegrationEventLogService>()
                .As<IIntegrationEventLogService>()
                .InstancePerDependency();

            builder.RegisterType<SK.IntegrationEventService>()
                .As<IIntegrationEventService>()
                .WithParameter(new TypedParameter(typeof(Assembly), typeof(MediatorModule).GetTypeInfo().Assembly))
                .WithParameter(new TypedParameter(typeof(string), MediatorModule.AppName))
                .InstancePerDependency();

            builder.RegisterType<IdpMailManager>()
                .As<IIdpMailManager>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SK.MailManager>()
                .As<IMailManager>()
                .InstancePerDependency();

            builder.RegisterType<SK.DateTimeService>()
                .As<IDateTime>()
                .InstancePerDependency();

            builder.RegisterInstance(_mailSettings);

            builder.RegisterInstance(_urlsSettings);

            builder.RegisterInstance(_administratorsOptions);
        }
    }
}