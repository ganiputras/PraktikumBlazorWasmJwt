using Application;
using Application.Contracts;
using Infrastructure;
using Infrastructure.Models;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Middleware;
using WebApi.Services;
using WebApi.Utility;

//using Serilog;

namespace WebApi
{
   public static class StartupExtensions
   {
      public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
      {
         // Add services to the container.

         AddSwagger(builder.Services);

         builder.Services.AddApplicationServices();
         //builder.Services.AddInfrastructureServices(builder.Configuration);
         //builder.Services.AddPersistenceServices(builder.Configuration);
         builder.Services.AddIdentityServices(builder.Configuration);

         builder.Services.AddScoped<ILoggedInUserService, LoggedInUserService>();

         builder.Services.AddHttpContextAccessor();

         builder.Services.AddControllers();

         builder.Services.AddCors(options =>
         {
            options.AddPolicy("Open", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
         });

         return builder.Build();

      }
      public static WebApplication ConfigurePipeline(this WebApplication app)
      {

         if (app.Environment.IsDevelopment())
         {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "Architecture API");
            });
         }

         app.UseHttpsRedirection();

         //app.UseRouting();

         app.UseAuthentication();

         app.UseCustomExceptionHandler();

         app.UseCors("Open");

         app.UseAuthorization();

         app.MapControllers();

         return app;

      }
      private static void AddSwagger(IServiceCollection services)
      {
         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
         services.AddEndpointsApiExplorer();

         services.AddSwaggerGen(c =>
         {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
               Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
               Name = "Authorization",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.ApiKey,
               Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
               {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                 });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
               Version = "v1",
               Title = "Architecture API",

            });

            c.OperationFilter<FileResultContentTypeOperationFilter>();
         });
      }
      public static async Task ResetDatabaseAsync(this WebApplication app)
      {
         if (app.Environment.IsDevelopment())
         {
            using var scope = app.Services.CreateScope();
            try
            {
               var ctxIdentity = scope.ServiceProvider.GetService<IdentityDbContext>();
               if (ctxIdentity != null)
               {
                  await ctxIdentity.Database.EnsureDeletedAsync();
                  await ctxIdentity.Database.MigrateAsync();
               }

               //seed user
               var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
               var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

               var userInitializer = scope.ServiceProvider.GetRequiredService<SeedUser>();
               await userInitializer.InitialiseAsync(userManager, roleManager);


            }
            catch (Exception ex)
            {
               var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
               logger.LogError(ex, "An error occurred while migrating the database.");
            }
         }
      }
   }
}