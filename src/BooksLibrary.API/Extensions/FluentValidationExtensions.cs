using BooksLibrary.Application.Common.Behaviors;
using BooksLibrary.Application.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using System.Reflection;

namespace BooksLibrary.API.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddApiValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(ApplicationMappingProfile).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>),
                                  typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
