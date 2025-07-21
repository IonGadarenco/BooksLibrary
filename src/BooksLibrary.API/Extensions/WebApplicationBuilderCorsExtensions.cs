namespace BooksLibrary.API.Extensions
{
    public static class WebApplicationBuilderCorsExtensions
    {
        public static WebApplicationBuilder RegisterCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return builder;
        }
    }
}
