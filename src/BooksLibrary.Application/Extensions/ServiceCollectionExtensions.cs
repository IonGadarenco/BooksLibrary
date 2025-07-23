using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BooksLibrary.Application.Common.Behaviors;

namespace BooksLibrary.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()));

            // scan for all AbstractValidator<T> in this assembly
            services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

            // hook up the pipeline
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)
            );

            return services;
        }
    }
}
