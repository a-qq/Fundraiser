using Autofac;
using FundraiserManagement.Application;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Infrastructure.Services;
using System.Reflection;
using FundraiserManagement.Application.Common.Interfaces.Services;

namespace FundraiserManagement.Infrastructure
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
                    typeof(Assembly), typeof(Fundraiser).GetTypeInfo().Assembly))
                .SingleInstance();

            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerDependency();

            builder.RegisterType<IntegrationEventService>()
                .As<IIntegrationEventService>()
                .WithParameter(new TypedParameter(typeof(Assembly), typeof(MediatorModule).GetTypeInfo().Assembly))
                .WithParameter(new TypedParameter(typeof(string), MediatorModule.AppName))
                .InstancePerDependency();
        }
    }
}