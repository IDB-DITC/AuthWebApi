using System.ComponentModel.DataAnnotations;

namespace AuthWebApi.Models
{
  public class UserRoleAssignDto
  {
    [Required]
    public string UserName { get; set; } = default!;
    public IList<string> Roles { get; set; } = [];
  }
}
