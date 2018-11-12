using System.Threading.Tasks;

namespace API.Models.UnitOfWork
{
  public interface IUnitOfWork
  {
    Task Commit();
  }
}