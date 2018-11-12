namespace API.Models.Entities
{
  public class Message
  {
    public int id { get; set; }
    public string Payload { get; set; }
    public AppUser User { get; set; }
    public string UserId { get; set; }
  }
}