using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Behaviours;

namespace SchoolManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSchoolManagementApplication(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(currentAssembly);

            services.AddInternalValidatorsFromAssembly(currentAssembly);
            services.AddMediatR(currentAssembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }

        private static void AddInternalValidatorsFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            new AssemblyScanner(assembly.GetTypes())
                .ForEach(pair => services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType)));
        }

        //private static void AddMediatBehaviours(this IServiceCollection services, Assembly assembly)
        //{
        //    var types = assembly.GetTypes()
        //        .Where(t => t.IsClass && !t.IsAbstract)
        //        .OrderBy(t => t.GetCustomAttributes(typeof(PipelinePriorityAttribute), true)
        //            .Cast<PipelinePriorityAttribute>()
        //            .FirstOrDefault()?.Priority ?? PipelinePriorityOrder.Normal);

        //    var pipelineType = typeof(IPipelineBehavior<,>);
        //    var gerenicPiplineType = pipelineType.MakeGenericType(pipelineType.GetGenericArguments());
        //    foreach (var type in types)
        //    {
        //        foreach (var i in type.GetInterfaces())
        //        {
        //            // Check for generic
        //            if (i.IsGenericType && i.GetGenericTypeDefinition() == pipelineType)
        //                services.Add(new ServiceDescriptor(gerenicPiplineType, type., ServiceLifetime.Transient));

        //            // Check for non-generic
        //            else if (!i.IsGenericType && i == pipelineType)
        //                services.Add(new ServiceDescriptor(pipelineType, type, ServiceLifetime.Transient));
        //        }
        //    }
        //}
    }
}