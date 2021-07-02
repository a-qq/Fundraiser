using Autofac;
using FluentValidation;
using FundraiserManagement.Application.Behaviors;
using MediatR;
using System.Reflection;
using SharedKernel.Infrastructure.Concretes.Services;

namespace FundraiserManagement.Application
{
    public sealed class MediatorModule : Autofac.Module
    {
        private static readonly string Namespace = typeof(MediatorModule).Namespace;
        private static readonly int Index = Namespace.IndexOf('.', Namespace.IndexOf('.') + 1);
        public static readonly string AppName = Namespace.Substring(0, Index < 0 ? Namespace.Length : Index);

        protected override void Load(ContainerBuilder builder)
        {
            

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            var mediatorOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
                typeof(IValidator<>),
            };

            foreach (var mediatorOpenType in mediatorOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(ThisAssembly)
                    .AsClosedTypesOf(mediatorOpenType)
                    .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(IdentifiedCommandHandler<>)).As(typeof(IRequestHandler<,>));

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => componentContext.Resolve(t);
            });

            builder.RegisterGeneric(typeof(UnhandledExceptionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(AuthorizationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(UserRequestValidationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(InternalRequestValidationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(UserRequestLoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(InternalQueryLoggingBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(InternalCommandLoggingBehaviour<>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}