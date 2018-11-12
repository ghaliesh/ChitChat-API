using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repositories
{
  public interface IMessageRepository
  {
    Task<Message> AddMessage(Message message);
  }
}