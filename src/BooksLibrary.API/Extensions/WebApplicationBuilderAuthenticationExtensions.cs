using BooksLibrary.Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BooksLibrary.API.Extensions
{
    public static class WebApplicationBuilderAuthenticationExtensions
    {
        public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSettings = new JwtSettings();
            builder.Configuration.Bind(nameof(JwtSettings), jwtSettings);
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwt =>
                {
                    jwt.Authority = jwtSettings.Issuer;
                    jwt.Audience = jwtSettings.Audiences[0];
                    jwt.MapInboundClaims = false;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audiences[0],
                        ValidateLifetime = true,
                        RoleClaimType = "https://bookslibrary.com/role"
                    };
                });

            return builder;
        }
    }
}
