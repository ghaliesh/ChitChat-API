using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repositories
{
  public class UserRepo : IUserRepo
  {
    private readonly APIDbContext context;

    public UserRepo(APIDbContext context)
    {
      this.context = context;
    }

    public AppUser GetUser(string userId)
    {
      var user = this.context.Users.SingleOrDefault(a => a.Id == userId);
      return user;
    }

    public AppUser WarnUser(AppUser user)
    {
      var warns = user.Warns += 1;
      if (warns > 2)
      {
        user = BanUser(user);
      }
      return user;
    }

    private AppUser BanUser(AppUser user)
    {
      user.Banned = true;
      return user;
    }
  }
}