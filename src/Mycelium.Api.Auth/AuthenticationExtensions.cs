using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mycelium.Api.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("User", policy => policy.RequireRole("User"));
            options.AddPolicy("Device", policy => policy.RequireRole("Device"));
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        var token = context.Request.Headers["Authorization"].ToString();
                        if (token.Contains("bearer"))
                        {
                            var jwtToken = token.Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
                            if (new JwtSecurityTokenHandler().ReadToken(jwtToken).ValidTo < DateTime.UtcNow)
                            {
                                await context.Response.WriteAsync("Expired JWT");
                                return;
                            }
                        }
                        await context.Response.WriteAsync("Invalid JWT");
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

        return services;
    }
}
