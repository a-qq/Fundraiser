using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Backend.API.Validators
{
    public static class DependecyInjection
    {
        public static void AddViewModelsValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
