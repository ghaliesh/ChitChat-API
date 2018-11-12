using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Entities
{
  public class APIDbContext : IdentityDbContext<AppUser>
  {
    public APIDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Message> Messages { get; set; }
  }
}