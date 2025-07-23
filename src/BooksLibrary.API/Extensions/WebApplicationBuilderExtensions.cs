namespace BooksLibrary.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApiServices(
          this WebApplicationBuilder builder)
        {
            builder.Services.AddApiValidation();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.RegisterAuthentication();
            builder.RegisterCors();
            builder.RegisterSwagger();
            return builder;
        }
    }
}
