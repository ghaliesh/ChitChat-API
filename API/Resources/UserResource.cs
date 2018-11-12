using System.ComponentModel.DataAnnotations;

namespace API.Resources
{
  public class UserResource
  {
    public string Name { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(255)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(1024)]
    public string Password { get; set; }
  }
}