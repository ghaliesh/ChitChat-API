using Microsoft.AspNetCore.Identity;

namespace API.Models.Entities
{
  public class AppUser : IdentityUser
  {
    public Photo Photo { get; set; }
    public int Warns { get; set; }
    public string Name { get; set; }
    public bool Banned { get; set; }
  }

}