using Jwt.Contexts;
using Jwt.Models;
using Jwt.Options;
using Jwt.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Jwt.Extensions
{
    public static class DependencyInjection //Extension olacağı için statik olmalı
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<JwtDbContext>(options => options.UseSqlite(configuration.GetConnectionString("Default")));

            //services.ConfigureOptions<JwtOptionsSetup>();

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));    //jwtoptions classına appsetting'de karşılık gelenlerle eşleştirmiş oluyorum.
            //services.ConfigureOptions<JwtBearerOptionsSetup>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //.AddJwtBearer();
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };
                });
            services.AddIdentity<AppUser,IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<JwtDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IJwtTokenProvider,JwtTokenProvider>();


            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
