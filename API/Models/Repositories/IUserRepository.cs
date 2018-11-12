using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repositories
{
  public interface IUserRepo
  {
    AppUser GetUser(string userId);
    AppUser WarnUser(AppUser user);
  }
}