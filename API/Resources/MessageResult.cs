using API.Models.Entities;

namespace API.Resources
{
  public class MessageResult
  {
    public string Payload { get; set; }
    public AppUser User { get; set; }
  }
}