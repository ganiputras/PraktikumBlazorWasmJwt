

using System.Text;
using System.Text.Json;
using Application.Contracts.Identity;
using Application.Models.Authentication;
using Infrastructure.Models;
using Infrastructure.Seed;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class IdentityServiceExtensions
{
   public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
   {
      services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

      services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(
         configuration.GetConnectionString("DefaultConnection"),
         b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

      services.AddIdentity<ApplicationUser, IdentityRole>()
         .AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

      services.AddTransient<IAppAuthenticationService, AppAuthenticationService>();

      services.AddAuthentication(options =>
         {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         })
         .AddJwtBearer(o =>
         {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateIssuerSigningKey = true,
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero,
               ValidIssuer = configuration["JwtSettings:Issuer"],
               ValidAudience = configuration["JwtSettings:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
            };

            o.Events = new JwtBearerEvents
            {
               OnAuthenticationFailed = c =>
               {
                  c.NoResult();
                  c.Response.StatusCode = 500;
                  c.Response.ContentType = "text/plain";
                  return c.Response.WriteAsync(c.Exception.ToString());
               },
               OnChallenge = context =>
               {
                  context.HandleResponse();
                  context.Response.StatusCode = 401;
                  context.Response.ContentType = "application/json";
                  var result = JsonSerializer.Serialize("401 Not authorized");
                  return context.Response.WriteAsync(result);
               },
               OnForbidden = context =>
               {
                  context.Response.StatusCode = 403;
                  context.Response.ContentType = "application/json";
                  var result = JsonSerializer.Serialize("403 Not authorized");
                  return context.Response.WriteAsync(result);
               }
            };
         });


      services.AddScoped<SeedUser>();
   }
}