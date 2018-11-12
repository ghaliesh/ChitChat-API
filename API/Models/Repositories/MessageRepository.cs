using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repositories
{
  public class MessageRepository : IMessageRepository
  {
    private readonly APIDbContext context;

    public MessageRepository(APIDbContext context)
    {
      this.context = context;
    }
    public async Task<Message> AddMessage(Message message)
    {
      var result = await context.AddAsync(message);
      return result.Entity;
    }

  }
}