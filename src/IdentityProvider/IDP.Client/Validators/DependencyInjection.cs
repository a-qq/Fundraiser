using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace IDP.Client.Validators
{
    public static class DependecyInjection
    {
        public static void AddViewModelsValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
