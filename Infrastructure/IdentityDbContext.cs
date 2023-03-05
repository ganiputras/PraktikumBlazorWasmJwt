using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
   public IdentityDbContext()
   {
   }

   public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
   {
   }

   //protected override void OnModelCreating(ModelBuilder modelBuilder)
   //{
   //   modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
   //}
}