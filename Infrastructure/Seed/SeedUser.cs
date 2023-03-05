

using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seed
{
    public class SeedUser
   {
      private static readonly string _password = "P@ssw0rd";

      public async Task InitialiseAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
      {

         //new
         string[] roles =
            {
              UserRoleType.SystemAdmin,
              UserRoleType.Admin,
              UserRoleType.User,
           };


         foreach (var role in roles)
            if (!roleManager.Roles.Any(r => r.Name == role))
               await roleManager.CreateAsync(new IdentityRole(role));


         var usrAdministrator = new ApplicationUser
         {
            UserName = "systemadmin",
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@gmail.com",
            PhoneNumber = "123456789000",
            EmailConfirmed = true
         };

         if (userManager.Users.All(u => u.UserName != usrAdministrator.UserName))
         {
            await userManager.CreateAsync(usrAdministrator, _password);
            await userManager.AddToRolesAsync(usrAdministrator, new[] { UserRoleType.SystemAdmin });
         }
      }
   }
}