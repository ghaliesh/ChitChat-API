using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.UnitOfWork
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly APIDbContext context;

    public UnitOfWork(APIDbContext context)
    {
      this.context = context;
    }
    public async Task Commit()
    {
      await context.SaveChangesAsync();
    }
  }
}