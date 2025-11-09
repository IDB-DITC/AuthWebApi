using System.ComponentModel.DataAnnotations;

namespace AuthWebApi.Models
{
  public class UserDto
  {
    [Required(AllowEmptyStrings = false)]
    public string UserName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
  }
}
